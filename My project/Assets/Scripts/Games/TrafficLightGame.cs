using System.Collections;
using UnityEngine;
using EduMotion.Core;
using EduMotion.Gestures;
using EduMotion.UI;

namespace EduMotion.Games
{
    public class TrafficLightGame : MonoBehaviour
    {
        public enum LightColor { Red, Yellow, Green }

        [SerializeField] private float _minLightDuration = 3f;
        [SerializeField] private float _maxLightDuration = 6f;
        [SerializeField] private int   _totalRounds      = 8;
        [SerializeField] private TrafficLightView _view;

        private LightColor _currentLight;
        private bool _waitingForGesture, _gameActive;

        private void Start() => StartCoroutine(RunGame());

        private void OnEnable()  => EventBus.Subscribe<GestureEvent>(GameEvents.GestureDetected, OnGesture);
        private void OnDisable() => EventBus.Unsubscribe<GestureEvent>(GameEvents.GestureDetected, OnGesture);

        private IEnumerator RunGame()
        {
            _gameActive = true;
            for (int r = 0; r < _totalRounds; r++)
            {
                _currentLight      = (LightColor)Random.Range(0, 3);
                _waitingForGesture = true;
                _view?.ShowLight(_currentLight);
                _view?.ShowInstruction(LightToInstruction(_currentLight));
                _view?.ShowFeedback(null);
                float timer = Random.Range(_minLightDuration, _maxLightDuration);
                float elapsed = 0f;
                while (elapsed < timer && _waitingForGesture)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                if (_waitingForGesture)
                {
                    _waitingForGesture = false;
                    RewardSystem.Instance?.Penalize();
                    _view?.ShowFeedback(false);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            _gameActive = false;
            EventBus.Publish(GameEvents.GameFinished);
        }

        private void OnGesture(GestureEvent evt)
        {
            if (!_gameActive || !_waitingForGesture) return;
            _waitingForGesture = false;
            bool ok = IsCorrectGesture(evt);
            if (ok) RewardSystem.Instance?.AddScore();
            else    RewardSystem.Instance?.Penalize();
            _view?.ShowFeedback(ok);
        }

        private bool IsCorrectGesture(GestureEvent evt) => _currentLight switch
        {
            LightColor.Green  => evt.Type == GestureType.RaiseHand && evt.Hand == HandSide.Right,
            LightColor.Yellow => evt.Type == GestureType.RaiseHand && evt.Hand == HandSide.Left,
            LightColor.Red    => evt.Type == GestureType.Stop || evt.Type == GestureType.Turn,
            _                 => false
        };

        private string LightToInstruction(LightColor c) => c switch
        {
            LightColor.Red    => "СТОП! Руки в стороны",
            LightColor.Yellow => "ЖДИ! Левая рука вверх",
            LightColor.Green  => "ИДИ! Правая рука вверх",
            _                 => ""
        };
    }
}
