using UnityEngine;
using UnityEngine.UI;

namespace EduMotion
{
    public class EcologyView : MonoBehaviour
    {
        [SerializeField] private Text _instructionText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _categoryText;

        public void SetInstruction(string text) => _instructionText.text = text;
        public void SetScore(int score) => _scoreText.text = "РћС‡РєРё: " + score;
        public void SetCategory(string category) => _categoryText.text = category;
    }
}
