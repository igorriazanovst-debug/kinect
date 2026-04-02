$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$scriptsUI = "$projectPath\Assets\Scripts\UI"

# ---- Fix MainMenuUI.cs ----
$mainMenuUI = @'
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EduMotion.Kinect;

namespace EduMotion.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _trafficButton;
        [SerializeField] private Button _crossingButton;
        [SerializeField] private Button _ecologyButton;

        [Header("Kinect Status")]
        [SerializeField] private Text _kinectStatusText;
        [SerializeField] private Image _kinectStatusIcon;
        [SerializeField] private Color _connectedColor    = Color.green;
        [SerializeField] private Color _disconnectedColor = Color.red;

        private void Start()
        {
            _trafficButton.onClick.AddListener(() => LoadScene("Game_Traffic"));
            _crossingButton.onClick.AddListener(() => LoadScene("Game_Crossing"));
            _ecologyButton.onClick.AddListener(() => LoadScene("Game_Ecology"));
        }

        private void Update()
        {
            bool connected = KinectManager.Instance != null && KinectManager.Instance.IsConnected;

            if (_kinectStatusText != null)
                _kinectStatusText.text = connected ? "Kinect: подключён" : "Kinect: не найден";

            if (_kinectStatusIcon != null)
                _kinectStatusIcon.color = connected ? _connectedColor : _disconnectedColor;

            _trafficButton.interactable  = connected;
            _crossingButton.interactable = connected;
            _ecologyButton.interactable  = connected;
        }

        private void LoadScene(string sceneName) => UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
'@

# ---- Fix TrafficLightView.cs ----
$trafficView = @'
using UnityEngine;
using UnityEngine.UI;
using EduMotion.Games;

namespace EduMotion.UI
{
    public class TrafficLightView : MonoBehaviour
    {
        [SerializeField] private Image _redLight;
        [SerializeField] private Image _yellowLight;
        [SerializeField] private Image _greenLight;
        [SerializeField] private Text  _instructionText;
        [SerializeField] private Text  _scoreText;
        [SerializeField] private Text  _feedbackText;

        private readonly Color _activeColor   = Color.white;
        private readonly Color _inactiveColor = new Color(1f, 1f, 1f, 0.2f);

        public void ShowLight(TrafficLightGame.LightColor color)
        {
            _redLight.color    = color == TrafficLightGame.LightColor.Red    ? _activeColor : _inactiveColor;
            _yellowLight.color = color == TrafficLightGame.LightColor.Yellow ? _activeColor : _inactiveColor;
            _greenLight.color  = color == TrafficLightGame.LightColor.Green  ? _activeColor : _inactiveColor;

            _instructionText.text = color switch
            {
                TrafficLightGame.LightColor.Red    => "Стой!",
                TrafficLightGame.LightColor.Yellow => "Подними руку!",
                TrafficLightGame.LightColor.Green  => "Шагай!",
                _                                  => ""
            };
        }

        public void ShowFeedback(bool correct)
        {
            if (_feedbackText == null) return;
            _feedbackText.text  = correct ? "Верно!" : "Ошибка!";
            _feedbackText.color = correct ? Color.green : Color.red;
        }

        public void SetScore(int score)
        {
            if (_scoreText != null) _scoreText.text = "Очки: " + score;
        }
    }
}
'@

Set-Content -Path "$scriptsUI\MainMenuUI.cs"       -Value $mainMenuUI  -Encoding UTF8
Set-Content -Path "$scriptsUI\TrafficLightView.cs" -Value $trafficView -Encoding UTF8

Write-Host "Fixed: MainMenuUI.cs, TrafficLightView.cs"
