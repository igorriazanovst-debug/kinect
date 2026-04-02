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
                _kinectStatusText.text = connected ? "Kinect: РїРѕРґРєР»СЋС‡С‘РЅ" : "Kinect: РЅРµ РЅР°Р№РґРµРЅ";

            if (_kinectStatusIcon != null)
                _kinectStatusIcon.color = connected ? _connectedColor : _disconnectedColor;

            _trafficButton.interactable  = connected;
            _crossingButton.interactable = connected;
            _ecologyButton.interactable  = connected;
        }

        private void LoadScene(string sceneName) => UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
