using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitChoiceButton : SkitSceneButtonBase
    {
        [SerializeField] private Image _buttonResultImage;
        [SerializeField] private TMP_Text _choiceText;
        
        public Image ButtonResultImage => _buttonResultImage;
        public void InitializeSkitChoiceButton(string choiceText, Sprite buttonResultImage)
        {
            _choiceText.text = choiceText;
            _buttonResultImage.sprite = buttonResultImage;
        }
    }
}
