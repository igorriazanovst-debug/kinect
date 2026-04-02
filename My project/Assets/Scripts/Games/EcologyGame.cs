using System.Collections;
using UnityEngine;
using EduMotion.Core;
using EduMotion.UI;
using EduMotion.Gestures;

namespace EduMotion.Games
{
    public enum TrashType { Organic, Plastic, Paper, Metal }

    [System.Serializable]
    public class TrashItem { public TrashType Type; public UnityEngine.Sprite Icon; }

    public class EcologyGame : MonoBehaviour
    {
        [SerializeField] private int       _totalItems  = 8;
        [SerializeField] private float     _itemTimeout = 6f;
        [SerializeField] private EcologyView _view;
        [SerializeField] private TrashItem[] _trashItems;

        private TrashType _currentTrash, _selectedBin;
        private bool _waitingConfirm, _gameActive;

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
            for (int i = 0; i < _totalItems; i++)
            {
                var item = _trashItems[Random.Range(0, _trashItems.Length)];
                _currentTrash = item.Type; _selectedBin = TrashType.Plastic; _waitingConfirm = true;
                _view?.ShowTrash(item); _view?.HighlightBin(_selectedBin);
                AudioManager.Instance?.PlayVoice("voice_sortgarbage");
                float elapsed = 0f;
                while (_waitingConfirm && elapsed < _itemTimeout) { elapsed += Time.deltaTime; yield return null; }
                if (_waitingConfirm) RewardSystem.Instance?.Penalize();
                _waitingConfirm = false;
                yield return new WaitForSeconds(0.5f);
            }
            _gameActive = false;
            EventBus.Publish(GameEvents.GameFinished);
        }

        private void OnGesture(GestureEvent evt)
        {
            if (!_gameActive) return;
            if (evt.Type == GestureType.Turn && _waitingConfirm)
            {
                _selectedBin = (TrashType)(((int)_selectedBin + 1) % 4);
                _view?.HighlightBin(_selectedBin);
            }
            if (evt.Type == GestureType.RaiseHand && _waitingConfirm)
            {
                bool ok = _selectedBin == _currentTrash;
                _waitingConfirm = false;
                if (ok) RewardSystem.Instance?.AddScore(); else RewardSystem.Instance?.Penalize();
                _view?.ShowFeedback(ok);
            }
        }
    }
}

