$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$scriptsUI = "$projectPath\Assets\Scripts\UI"

if (-not (Test-Path $scriptsUI)) {
    New-Item -ItemType Directory -Path $scriptsUI -Force | Out-Null
}

$mainMenuUIContent = @'
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace EduMotion
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
        [SerializeField] private Color _connectedColor = Color.green;
        [SerializeField] private Color _disconnectedColor = Color.red;

        private KinectManager _kinectManager;

        private void Awake()
        {
            _kinectManager = FindObjectOfType<KinectManager>();
        }

        private void Start()
        {
            _trafficButton.onClick.AddListener(() => LoadScene("Game_Traffic"));
            _crossingButton.onClick.AddListener(() => LoadScene("Game_Crossing"));
            _ecologyButton.onClick.AddListener(() => LoadScene("Game_Ecology"));
        }

        private void Update()
        {
            UpdateKinectStatus();
        }

        private void UpdateKinectStatus()
        {
            if (_kinectManager == null) return;

            bool connected = _kinectManager.IsConnected;
            if (_kinectStatusText != null)
                _kinectStatusText.text = connected ? "Kinect: подключён" : "Kinect: не найден";

            if (_kinectStatusIcon != null)
                _kinectStatusIcon.color = connected ? _connectedColor : _disconnectedColor;

            _trafficButton.interactable = connected;
            _crossingButton.interactable = connected;
            _ecologyButton.interactable = connected;
        }

        private void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
'@

Set-Content -Path "$scriptsUI\MainMenuUI.cs" -Value $mainMenuUIContent -Encoding UTF8
Write-Host "MainMenuUI.cs создан: $scriptsUI\MainMenuUI.cs"
