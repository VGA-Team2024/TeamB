using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private TMP_Text _talkerNameText;
        [SerializeField] private GameObject _talkerNamePanel;
        [SerializeField] private float _textSpeed = 0.1f;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private SkitResourceLoader _skitResourceLoader;
        [SerializeField] private Image _leftCharaImage;
        [SerializeField] private Image _rightCharaImage;
        [SerializeField] private Image _middleCharaImage;
        private CancellationTokenSource _talkCancellationTokenSource;
        
        public void SetSkitResourceLoader(SkitResourceLoader skitResourceLoader)
        {
            _skitResourceLoader = skitResourceLoader;
        }

        public void SetCharacterAndBackground(string backgroundName, SkitTalkCharaData[] talkCharaData)
        {
            _backgroundImage.sprite = _skitResourceLoader.GetSpriteByName(backgroundName);
            _rightCharaImage.gameObject.SetActive(false);
            _leftCharaImage.gameObject.SetActive(false);
            _middleCharaImage.gameObject.SetActive(false);
            foreach (var charaData in talkCharaData)
            {
                var charaImage = charaData.StandingPosition switch
                {
                    StandingPosition.Left => _leftCharaImage,
                    StandingPosition.Right => _rightCharaImage,
                    StandingPosition.Middle => _middleCharaImage,
                    _ => null
                };
                
                if (charaImage == null)
                {
                    Debug.LogWarning("Invalid standing position");
                    continue;
                }
                charaImage.sprite = _skitResourceLoader.GetSpriteByName(charaData.CharaStateFileName);
                charaImage.gameObject.SetActive(true);
            }
        }
        
        public async UniTask ShowDialogue(string talkerName, string dialogue)
        {
            _talkCancellationTokenSource?.Cancel();
            _talkCancellationTokenSource = new CancellationTokenSource();
            if (string.IsNullOrEmpty(talkerName))
            {
                _talkerNamePanel.SetActive(false);
            }
            else
            {
                _talkerNamePanel.SetActive(true);
                _talkerNameText.text = talkerName;
            }
            _dialogueText.text = "";
            foreach (var c in dialogue)
            {
                _dialogueText.text += c;
                await UniTask.WaitForSeconds(_textSpeed, cancellationToken: _talkCancellationTokenSource.Token);
            }
        }
    }
}
