using System.Collections;
using UnityEngine;
using EduMotion.Core;
using EduMotion.Gestures;

namespace EduMotion.Games
{
    public class CrossingGame : MonoBehaviour
    {
        [SerializeField] private int   _totalCrossings = 5;
        [SerializeField] private float _dangerWindow   = 1.5f;
        [SerializeField] private float _safeWindow     = 2.0f;
        [SerializeField] private CrossingView _view;

        private bool _isSafe, _waitingInput, _gameActive;

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
            for (int i = 0; i < _totalCrossings; i++)
            {
                _isSafe = false; _waitingInput = true;
                _view?.ShowDanger(true);
                AudioManager.Instance?.PlayVoice("voice_wait");
                yield return new WaitForSeconds(_dangerWindow);
                _isSafe = true;
                _view?.ShowDanger(false);
                AudioManager.Instance?.PlayVoice("voice_go");
                yield return new WaitForSeconds(_safeWindow);
                if (_waitingInput) { RewardSystem.Instance?.Penalize(); _view?.ShowFeedback(false); yield return new WaitForSeconds(0.6f); }
                _waitingInput = false;
            }
            _gameActive = false;
            EventBus.Publish(GameEvents.GameFinished);
        }

        private void OnGesture(GestureEvent evt)
        {
            if (!_gameActive || !_waitingInput || evt.Type != GestureType.StepForward) return;
            _waitingInput = false;
            if (_isSafe) RewardSystem.Instance?.AddScore(); else RewardSystem.Instance?.Penalize(15);
            _view?.ShowFeedback(_isSafe);
        }
    }
}
