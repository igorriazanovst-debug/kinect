using UnityEngine;
using UnityEngine.UI;
using EduMotion.Games;
namespace EduMotion.UI
{
    public class TrafficLightView : MonoBehaviour
    {
        [SerializeField] private Image _lightImage;
        [SerializeField] private Text  _instructionText;
        [SerializeField] private Text  _feedbackText;
        [SerializeField] private Color _colorRed = Color.red, _colorYellow = Color.yellow, _colorGreen = Color.green;

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

        public void ShowInstruction(string text)
        {
            if (_instructionText != null) _instructionText.text = text;
        }

        public void ShowFeedback(bool? ok)
        {
            if (_feedbackText == null) return;
            if (ok == null)  { _feedbackText.text = ""; return; }
            _feedbackText.text  = ok.Value ? "CORRECT!" : "WRONG!";
            _feedbackText.color = ok.Value ? Color.green  : Color.red;
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 1f);
        }

        private void ClearFeedback() { if (_feedbackText != null) _feedbackText.text = ""; }
    }

    public class CrossingView : MonoBehaviour
    {
        [SerializeField] private GameObject _dangerIndicator;
        [SerializeField] private Text _feedbackText;
        public void ShowDanger(bool d) => _dangerIndicator?.SetActive(d);
        public void ShowFeedback(bool? ok)
        {
            if (_feedbackText == null) return;
            if (ok == null) { _feedbackText.text = ""; return; }
            _feedbackText.text  = ok.Value ? "CORRECT!" : "WRONG!";
            _feedbackText.color = ok.Value ? Color.green : Color.red;
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 1f);
        }
        private void ClearFeedback() { if (_feedbackText != null) _feedbackText.text = ""; }
    }

    public class EcologyView : MonoBehaviour
    {
        [SerializeField] private Image      _trashImage;
        [SerializeField] private Text       _feedbackText;
        [SerializeField] private GameObject[] _binHighlights;
        public void ShowTrash(TrashItem item) { if (_trashImage) _trashImage.sprite = item.Icon; }
        public void HighlightBin(TrashType t) { for(int i=0;i<_binHighlights.Length;i++) _binHighlights[i]?.SetActive(i==(int)t); }
        public void ShowFeedback(bool? ok)
        {
            if (_feedbackText == null) return;
            if (ok == null) { _feedbackText.text = ""; return; }
            _feedbackText.text  = ok.Value ? "CORRECT!" : "WRONG!";
            _feedbackText.color = ok.Value ? Color.green : Color.red;
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 1f);
        }
        private void ClearFeedback() { if (_feedbackText != null) _feedbackText.text = ""; }
    }
}
