using UnityEngine;
using UnityEngine.UI;
using EduMotion.Core;
using EduMotion.Games;

namespace EduMotion.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _btnTraffic, _btnCrossing, _btnEcology;
        [SerializeField] private string _trafficScene="Game_Traffic", _crossingScene="Game_Crossing", _ecologyScene="Game_Ecology";
        [SerializeField] private Text _kinectStatus;

        private void Start()
        {
            _btnTraffic?.onClick.AddListener(() => GameManager.Instance?.StartGame(_trafficScene));
            _btnCrossing?.onClick.AddListener(() => GameManager.Instance?.StartGame(_crossingScene));
            _btnEcology?.onClick.AddListener(() => GameManager.Instance?.StartGame(_ecologyScene));
            EventBus.Subscribe(GameEvents.KinectConnected,    () => { if(_kinectStatus) _kinectStatus.text="Kinect: подключён ✓"; });
            EventBus.Subscribe(GameEvents.KinectDisconnected, () => { if(_kinectStatus) _kinectStatus.text="Kinect: не найден"; });
        }
    }
}
