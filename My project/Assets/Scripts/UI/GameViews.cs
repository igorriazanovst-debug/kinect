using UnityEngine;
using UnityEngine.UI;
using EduMotion.Games;

namespace EduMotion.UI
{
    public class TrafficLightView : MonoBehaviour
    {
        [SerializeField] private Image _lightImage;
        [SerializeField] private Color _colorRed = Color.red, _colorYellow = Color.yellow, _colorGreen = Color.green;
        [SerializeField] private GameObject _feedbackOk, _feedbackFail;

        public void ShowLight(TrafficLightGame.LightColor color)
        {
            if (_lightImage == null) return;
            _lightImage.color = color switch
            {
                TrafficLightGame.LightColor.Red    => _colorRed,
                TrafficLightGame.LightColor.Yellow => _colorYellow,
                TrafficLightGame.LightColor.Green  => _colorGreen,
                _                                  => Color.white
            };
        }

        public void ShowFeedback(bool ok) { _feedbackOk?.SetActive(ok); _feedbackFail?.SetActive(!ok); Invoke(nameof(Hide),1f); }
        private void Hide() { _feedbackOk?.SetActive(false); _feedbackFail?.SetActive(false); }
    }

    public class CrossingView : MonoBehaviour
    {
        [SerializeField] private GameObject _dangerIndicator, _feedbackOk, _feedbackFail;
        public void ShowDanger(bool d) => _dangerIndicator?.SetActive(d);
        public void ShowFeedback(bool ok) { _feedbackOk?.SetActive(ok); _feedbackFail?.SetActive(!ok); Invoke(nameof(Hide),1f); }
        private void Hide() { _feedbackOk?.SetActive(false); _feedbackFail?.SetActive(false); }
    }

    public class EcologyView : MonoBehaviour
    {
        [SerializeField] private Image      _trashImage;
        [SerializeField] private GameObject _feedbackOk, _feedbackFail;
        [SerializeField] private GameObject[] _binHighlights;

        public void ShowTrash(TrashItem item) { if (_trashImage) _trashImage.sprite = item.Icon; }
        public void HighlightBin(TrashType t) { for(int i=0;i<_binHighlights.Length;i++) _binHighlights[i]?.SetActive(i==(int)t); }
        public void ShowFeedback(bool ok) { _feedbackOk?.SetActive(ok); _feedbackFail?.SetActive(!ok); Invoke(nameof(Hide),1f); }
        private void Hide() { _feedbackOk?.SetActive(false); _feedbackFail?.SetActive(false); }
    }
}
