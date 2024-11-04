using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using TMPro;
using UnityEngine;

namespace TeamB.ConversationSystem
{
    public interface IConversationView
    {
        void SetSpeakerText(string speaker);
        void SetDialogueText(string dialogue);
        void SetCharacterData(CharacterData characterData);
    }
    
    [System.Serializable]
    public class ConversationView : IConversationView
    {
        [SerializeField] private TextMeshProUGUI _speakerText;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private List<TestCharaImage> _charaImages;
        
        public void SetSpeakerText(string speaker)
        {
            _speakerText.text = speaker;
        }
        
        public void SetDialogueText(string dialogue)
        {
            _charaImages.ForEach(x =>
            {
                x.CharaName.text = "";
                x.CharaAnimation.text = "";
                x.gameObject.SetActive(false);
            });
            _dialogueText.text = dialogue;
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
