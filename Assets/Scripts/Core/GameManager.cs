using UnityEngine;
using UnityEngine.SceneManagement;
using EduMotion.Core;
using EduMotion.Gestures;

namespace EduMotion.Games
{
    public enum GameState { MainMenu, Playing, Paused, Finished }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; } = GameState.MainMenu;

        [SerializeField] private string _mainMenuScene  = "MainMenu";
        [SerializeField] private string _trafficScene   = "Game_Traffic";
        [SerializeField] private string _crossingScene  = "Game_Crossing";
        [SerializeField] private string _ecologyScene   = "Game_Ecology";

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<string>(GameEvents.SceneChangeRequest, LoadScene);
            EventBus.Subscribe(GameEvents.GameFinished, OnGameFinished);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<string>(GameEvents.SceneChangeRequest, LoadScene);
            EventBus.Unsubscribe(GameEvents.GameFinished, OnGameFinished);
        }

        public void StartGame(string sceneName)
        {
            State = GameState.Playing;
            EventBus.Publish(GameEvents.GameStarted, sceneName);
            LoadScene(sceneName);
        }

        public void ReturnToMenu()
        {
            State = GameState.MainMenu;
            LoadScene(_mainMenuScene);
        }

        private void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
        private void OnGameFinished() => State = GameState.Finished;
    }
}
