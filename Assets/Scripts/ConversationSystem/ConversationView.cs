using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TeamB.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.ConversationSystem
{
    public interface IConversationView
    {
        void OnOpenConversationView();
        void OnCloseConversationView();
        void SetSpeakerText(string speaker);
        UniTask SetDialogueText(string dialogue);
        void SetCharacterData(CharacterData characterData);
        void ResetCharaImages();
        void SetBackground(Sprite background);
    }
    
    [System.Serializable]
    public class ConversationView : IConversationView
    {
        [SerializeField] private TextMeshProUGUI _speakerText;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private List<TestCharaImage> _charaImages;
        [SerializeField] private float _textSpeed = 0.1f;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private List<GameObject> _conversationViewObjects;

        public void OnOpenConversationView()
        {
            _conversationViewObjects.ForEach(x => x.SetActive(true));
        }

        public void OnCloseConversationView()
        {
            _conversationViewObjects.ForEach(x => x.SetActive(false));
        }

        public void SetSpeakerText(string speaker)
        {
            _speakerText.text = speaker;
        }
        
        public void ResetCharaImages()
        {
            _charaImages.ForEach(x =>
            {
                x.CharaName.text = "";
                x.CharaAnimation.text = "";
                x.gameObject.SetActive(false);
            });
        }

        public void SetBackground(Sprite background)
        {
            _backgroundImage.sprite = background;
        }

        public async UniTask SetDialogueText(string dialogue)
        {
            _dialogueText.text = "";
            foreach (var c in dialogue)
            {
                _dialogueText.text += c;
                await UniTask.Delay(TimeSpan.FromSeconds(_textSpeed)); // Adjust the interval as needed
            }
        }
        
        public void SetCharacterData(CharacterData characterData)
        {
            //Todo: 画像をセットする処理を追加
            switch (characterData.Position)
            {
                case Position.Left:
                    _charaImages[0].gameObject.SetActive(true);
                    _charaImages[0].CharaName.text = characterData.CharacterName;
                    _charaImages[0].CharaAnimation.text = characterData.Animation;
                    break;
                case Position.Middle:
                    _charaImages[1].gameObject.SetActive(true);
                    _charaImages[1].CharaName.text = characterData.CharacterName;
                    _charaImages[1].CharaAnimation.text = characterData.Animation;
                    break;
                case Position.Right:
                    _charaImages[2].gameObject.SetActive(true);
                    _charaImages[2].CharaName.text = characterData.CharacterName;
                    _charaImages[2].CharaAnimation.text = characterData.Animation;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
