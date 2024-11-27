using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitChoiceButton : MonoBehaviour
    {
        [SerializeField] private Image _buttonResultImage;
        [SerializeField] private TMP_Text _choiceText;
        [SerializeField] private Button _choiceButton;
        
        public Button ChoiceButton => _choiceButton;
        public Image ButtonResultImage => _buttonResultImage;
        public void InitializeSkitChoiceButton(string choiceText, Sprite buttonResultImage)
        {
            _choiceText.text = choiceText;
            _buttonResultImage.sprite = buttonResultImage;
        }
    }
}
