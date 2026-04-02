using UnityEngine;
using EduMotion.Core;

namespace EduMotion.Games
{
    public class RewardData
    {
        public int   Stars;
        public int   Score;
        public float TimeElapsed;
    }

    public class RewardSystem : MonoBehaviour
    {
        public static RewardSystem Instance { get; private set; }
        public int   Score       { get; private set; }
        public int   Stars       { get; private set; }
        public float TimeElapsed { get; private set; }

        [SerializeField] private int   _scorePerCorrect = 10;
        [SerializeField] private float _timeLimitSec    = 60f;
        private bool _running;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            EventBus.Subscribe(GameEvents.GameStarted, OnGameStarted);
            EventBus.Subscribe(GameEvents.GameFinished, OnGameFinished);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(GameEvents.GameStarted, OnGameStarted);
            EventBus.Unsubscribe(GameEvents.GameFinished, OnGameFinished);
        }

        private void Update() { if (_running) TimeElapsed += Time.deltaTime; }

        public void AddScore(int points = 0) => Score += points > 0 ? points : _scorePerCorrect;
        public void Penalize(int points = 5) => Score = Mathf.Max(0, Score - points);

        public RewardData Finalize()
        {
            _running = false;
            Stars = CalculateStars();
            var data = new RewardData { Stars = Stars, Score = Score, TimeElapsed = TimeElapsed };
            EventBus.Publish(GameEvents.RewardGranted, data);
            return data;
        }

        private void OnGameStarted() { Score = 0; TimeElapsed = 0f; Stars = 0; _running = true; }
        private void OnGameFinished() => Finalize();

        private int CalculateStars()
        {
            float ratio = Score / (float)(_timeLimitSec * _scorePerCorrect / 10f);
            if (ratio >= 0.8f) return 3;
            if (ratio >= 0.5f) return 2;
            return 1;
        }
    }
}
