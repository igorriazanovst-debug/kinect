using System.Collections;
using UnityEngine;
using EduMotion.Core;
using EduMotion.UI;
using EduMotion.Gestures;

namespace EduMotion.Games
{
    public class TrafficLightGame : MonoBehaviour
    {
        public enum LightColor { Red, Yellow, Green }

        [SerializeField] private float _minLightDuration = 2f;
        [SerializeField] private float _maxLightDuration = 5f;
        [SerializeField] private int   _totalRounds      = 8;
        [SerializeField] private TrafficLightView _view;

        private LightColor _currentLight;
        private bool _waitingForGesture, _gameActive;

        private void OnEnable()
        {
            EventBus.Subscribe<GestureEvent>(GameEvents.GestureDetected, OnGesture);
            EventBus.Subscribe(GameEvents.GameStarted, OnGameStarted);
        }
        private void OnDisable()
        {
            EventBus.Unsubscribe<GestureEvent>(GameEvents.GestureDetected, OnGesture);
            EventBus.Unsubscribe(GameEvents.GameStarted, OnGameStarted);
        }

        private void OnGameStarted() => StartCoroutine(RunGame());

        private IEnumerator RunGame()
        {
            _gameActive = true;
            for (int r = 0; r < _totalRounds; r++)
            {
                _currentLight      = (LightColor)Random.Range(0, 3);
                _waitingForGesture = true;
                _view?.ShowLight(_currentLight);
                AudioManager.Instance?.PlayVoice(LightToVoiceKey(_currentLight));
                yield return new WaitForSeconds(Random.Range(_minLightDuration, _maxLightDuration));
                if (_waitingForGesture) { RewardSystem.Instance?.Penalize(); _view?.ShowFeedback(false); yield return new WaitForSeconds(0.8f); }
            }
            _gameActive = false;
            EventBus.Publish(GameEvents.GameFinished);
        }

        private void OnGesture(GestureEvent evt)
        {
            if (!_gameActive || !_waitingForGesture) return;
            _waitingForGesture = false;
            bool ok = IsCorrectGesture(evt.Type);
            if (ok) RewardSystem.Instance?.AddScore(); else RewardSystem.Instance?.Penalize();
            _view?.ShowFeedback(ok);
        }

        private bool IsCorrectGesture(GestureType g) => _currentLight switch
        {
            LightColor.Green  => g == GestureType.StepForward,
            LightColor.Red    => g == GestureType.Stop,
            LightColor.Yellow => g == GestureType.RaiseHand,
            _                 => false
        };

        private string LightToVoiceKey(LightColor c) => c switch
        {
            LightColor.Red    => "voice_red",
            LightColor.Yellow => "voice_yellow",
            LightColor.Green  => "voice_green",
            _                 => ""
        };
    }
}

