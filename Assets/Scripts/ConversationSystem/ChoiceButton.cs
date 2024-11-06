using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.ConversationSystem
{
    public class ChoiceButton : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private Image _image = null;
        public Button Button => _button;
        
        public void SetText(string text)
        {
            _text.text = text;
        }
        
        public void SetIcon(Sprite icon)
        {
            _image.sprite = icon;
        }
    }
}

