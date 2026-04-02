using UnityEngine;
using UnityEngine.UI;

namespace EduMotion
{
    public class CrossingView : MonoBehaviour
    {
        [SerializeField] private Text _instructionText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Slider _progressBar;

        public void SetInstruction(string text) => _instructionText.text = text;
        public void SetScore(int score) => _scoreText.text = "РћС‡РєРё: " + score;
        public void SetProgress(float value) => _progressBar.value = value;
    }
}
