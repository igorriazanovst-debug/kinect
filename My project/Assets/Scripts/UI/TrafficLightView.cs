using UnityEngine;
using UnityEngine.UI;

namespace EduMotion
{
    public class TrafficLightView : MonoBehaviour
    {
        [SerializeField] private Image _redLight;
        [SerializeField] private Image _yellowLight;
        [SerializeField] private Image _greenLight;
        [SerializeField] private Text _instructionText;
        [SerializeField] private Text _scoreText;

        private Color _activeColor = Color.white;
        private Color _inactiveColor = new Color(1f, 1f, 1f, 0.2f);

        public void SetLight(TrafficLightColor color)
        {
            _redLight.color    = color == TrafficLightColor.Red    ? _activeColor : _inactiveColor;
            _yellowLight.color = color == TrafficLightColor.Yellow ? _activeColor : _inactiveColor;
            _greenLight.color  = color == TrafficLightColor.Green  ? _activeColor : _inactiveColor;
        }

        public void SetInstruction(string text) => _instructionText.text = text;
        public void SetScore(int score) => _scoreText.text = "РћС‡РєРё: " + score;
    }
}
