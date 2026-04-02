using UnityEngine;
using UnityEngine.UI;
using EduMotion.Core;
using EduMotion.Games;

namespace EduMotion.UI
{
    public class RewardScreenUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _scoreText, _timeText;
        [SerializeField] private Image[] _stars;
        [SerializeField] private Sprite _starFilled, _starEmpty;
        [SerializeField] private Button _btnMenu, _btnReplay;
        [SerializeField] private string _replayScene;

        private void Start()
        {
            _panel?.SetActive(false);
            _btnMenu?.onClick.AddListener(() => GameManager.Instance?.ReturnToMenu());
            _btnReplay?.onClick.AddListener(() => GameManager.Instance?.StartGame(_replayScene));
            EventBus.Subscribe<RewardData>(GameEvents.RewardGranted, Show);
        }

        private void OnDestroy() => EventBus.Unsubscribe<RewardData>(GameEvents.RewardGranted, Show);

        private void Show(RewardData d)
        {
            _panel?.SetActive(true);
            if (_scoreText) _scoreText.text = $"Очки: {d.Score}";
            if (_timeText)  _timeText.text  = $"Время: {d.TimeElapsed:F1} сек";
            for (int i=0;i<_stars.Length;i++) if(_stars[i]) _stars[i].sprite = i<d.Stars ? _starFilled : _starEmpty;
        }
    }
}
