using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using TeamB.GameSystem.Statics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitView : MonoBehaviour
    {
        [Header("会話表示関連")]
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private TMP_Text _talkerNameText;
        [SerializeField] private GameObject _talkerNamePanel;
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField, Range(0, 0.2f)] private float _textSpeed = 0.03f;
        [Header("背景・キャラ表示関連")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _leftCharaImage;
        [SerializeField] private Image _rightCharaImage;
        [SerializeField] private Image _middleCharaImage;
        [Header("授業選択肢表示関連")]
        [SerializeField] private GameObject _classSelectPanel;
        [SerializeField] private Transform _classSelectButtonParent;
        [SerializeField] private ClassSelectButton _classSelectButtonPrefab;
        [SerializeField] private TMP_Text _restDayText;
        [Header("選択肢表示関連")]
        [SerializeField] private SkitChoiceButton _choiceButtonPrefab;
        [SerializeField] private Sprite _correctChoiceSprite;
        [SerializeField] private Sprite _missChoiceSprite;
        [SerializeField] private Transform _choiceButtonParent;
        [SerializeField] private GameObject _restTimePanel;
        [SerializeField] private TMP_Text _restTimeText;
        [SerializeField] private GameObject _statusPanel;
        [SerializeField] private TMP_Text _intuitionText;
        [SerializeField] private TMP_Text _readingComprehensionText;
        [SerializeField] private TMP_Text _concentrationText;
        [Header("チュートリアル用")]
        [SerializeField] private GameObject _tutorialPanelAboutGame;
        [SerializeField] private GameObject _tutorialPanelAboutClassSelect;
        [SerializeField] private GameObject _tutorialPanelAboutSkitChoice;
        [SerializeField] private GameObject _tutorialPanelAboutSkitResult;
        private SkitResourceLoader _skitResourceLoader;
        private string _parameterPoint = "F1";
        
        public void InitializeSkitView(SkitResourceLoader skitResourceLoader)
        {
            _skitResourceLoader = skitResourceLoader;
            var key = (int)GameStatics.NurturingCharacterType;
            if (GameStatics.Characters.ContainsKey(key))
            {
                Observable.EveryUpdate().Subscribe(_ =>
                {
                    _intuitionText.text =  GameStatics.Characters[(int) GameStatics.NurturingCharacterType].MagicATK.ToString(_parameterPoint);
                    _readingComprehensionText.text = GameStatics.Characters[(int) GameStatics.NurturingCharacterType].ChantingSpeed.ToString(_parameterPoint);
                    _concentrationText.text = GameStatics.Characters[(int) GameStatics.NurturingCharacterType].HitRate.ToString(_parameterPoint);
                }).AddTo(this);
            }
        }
        
        public async UniTask ShowTutorialAboutGame(NormalTutorialData tutorialData, UniTaskCompletionSource emptyInput, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            SetCharacterAndBackground(tutorialData.BackgroundImageName, null);
            _tutorialPanelAboutGame.SetActive(true);
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken : cancellationToken);
            emptyInput.TrySetResult();
            _tutorialPanelAboutGame?.SetActive(false);
        }
        
        public void ShowTutorialAboutClassSelect(TutorialClassSelectData tutorialData, UniTaskCompletionSource<string> awaitSelect, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            SetCharacterAndBackground(tutorialData.BackgroundImageName, null);
            _classSelectPanel.SetActive(true);
            _tutorialPanelAboutClassSelect.SetActive(true);
            foreach (Transform child in _classSelectButtonParent)
            {
                Destroy(child.gameObject);
            }
            foreach (var classSelectEntry in tutorialData.ClassChoices)
            {
                var button = Instantiate(_classSelectButtonPrefab, _classSelectButtonParent);
                button.InitializeClassSelectButton(classSelectEntry.ChoiceName, classSelectEntry.TalkReward);
                button.ClassSelectButtonComponent.onClick.AddListener(() =>
                {
                    awaitSelect.TrySetResult(classSelectEntry.TalkDataId);
                    _classSelectPanel.SetActive(false);
                    _tutorialPanelAboutClassSelect?.SetActive(false);
                });
            }
        }
        
        public async UniTask ShowTutorialAboutSkitChoice(TutorialChoiceData tutorialChoiceData, UniTaskCompletionSource<string> awaitSelect, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            SetCharacterAndBackground(tutorialChoiceData.TalkBackground, null);
            _tutorialPanelAboutSkitChoice.SetActive(true);
            _choiceButtonParent.gameObject.SetActive(true);
            _statusPanel.SetActive(true);
            _restTimePanel.SetActive(true);
            await ShowDialogue(tutorialChoiceData.TalkSpeaker, tutorialChoiceData.JapaneseTalkDialogue, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Operation was cancelled.");
                return;
            }
            foreach (Transform child in _choiceButtonParent)
            {
                Destroy(child.gameObject);
            }
            Debug.Log("ShowTutorialAboutSkitChoice");
            foreach (var choiceEntry in tutorialChoiceData.ChoiceEntries)
            {
                var button = Instantiate(_choiceButtonPrefab, _choiceButtonParent);
                var resultSprite = _missChoiceSprite;
                if (string.Equals(choiceEntry.EnglishChoiceEntryName, tutorialChoiceData.Answer))
                {
                    resultSprite = _correctChoiceSprite;
                }
                button.InitializeSkitChoiceButton( choiceEntry.JapaneseChoiceEntryName, resultSprite);
            
                button.ChoiceButton.onClick.AddListener(() =>
                {
                    awaitSelect.TrySetResult(tutorialChoiceData.Answer);
                    LockAndShowAllChoiceButtonsResult();
                    button.ButtonResultImage.gameObject.SetActive(true);
                });
                button.ButtonResultImage.gameObject.SetActive(false);
            }
        }
        
        public async UniTask ShowTutorialAboutSkitResult(NormalTutorialData tutorialData, UniTaskCompletionSource awaitForEmptyInput, CancellationToken cancellationToken)
        {
            SetCharacterAndBackground(tutorialData.BackgroundImageName, null);
            _tutorialPanelAboutSkitChoice.SetActive(false);
            _tutorialPanelAboutSkitResult.SetActive(true);
            _statusPanel.SetActive(true);
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken : cancellationToken);
            awaitForEmptyInput.TrySetResult();
            _tutorialPanelAboutSkitResult?.SetActive(false);
        }

        public async UniTask ShowClassSelect(ClassSelectData classSelectData, UniTaskCompletionSource<string> awaitSelect, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            SetCharacterAndBackground(classSelectData.BackgroundImageName, null);
            await ShowDialogue(classSelectData.TalkerName, classSelectData.Dialogue, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Operation was cancelled.");
                return;
            }
            _classSelectPanel.SetActive(true);
            _restDayText.text = $"残り{GameStatics.RemainingDayForExam}日";
            foreach (Transform child in _classSelectButtonParent)
            {
                Destroy(child.gameObject);
            }
            foreach (var classSelectEntry in classSelectData.ClassChoices)
            {
                var button = Instantiate(_classSelectButtonPrefab, _classSelectButtonParent);
                button.InitializeClassSelectButton(classSelectEntry.ChoiceName, classSelectEntry.TalkReward);
                button.ClassSelectButtonComponent.onClick.AddListener(() =>
                {
                    awaitSelect.TrySetResult(classSelectEntry.TalkDataId);
                    _classSelectPanel.SetActive(false);
                });
            }
        }
     
        public async UniTask ShowSkitChoice(SkitChoiceData skitChoiceData, UniTaskCompletionSource<string> awaitChoice, UniTaskCompletionSource awaitEmptyInput, float time, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            SetCharacterAndBackground(skitChoiceData.TalkBackground, skitChoiceData.TalkCharaData);
            await ShowDialogue(skitChoiceData.TalkSpeaker, skitChoiceData.JapaneseTalkDialogue, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Operation was cancelled.");
                return;
            }
            _statusPanel.SetActive(true);   //ステータス表示
            _restTimePanel.SetActive(true); //残り時間設定
            UpdateRestTimeAsync(time, awaitChoice, awaitEmptyInput, cancellationToken).Forget();
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Operation was cancelled.");
                return;
            }
            _choiceButtonParent.gameObject.SetActive(true); //選択肢設定
            foreach (Transform child in _choiceButtonParent)
            {
                Destroy(child.gameObject);
            }
            foreach (var choiceEntry in skitChoiceData.ChoiceEntries)
            {
                var button = Instantiate(_choiceButtonPrefab, _choiceButtonParent);
                var resultSprite = _missChoiceSprite;
                if (string.Equals(choiceEntry.EnglishChoiceEntryName, skitChoiceData.Answer))
                {
                    resultSprite = _correctChoiceSprite;
                }
                button.InitializeSkitChoiceButton( choiceEntry.JapaneseChoiceEntryName, resultSprite);
            
                button.ChoiceButton.onClick.AddListener( async () =>
                {
                    awaitChoice.TrySetResult(choiceEntry.EnglishChoiceEntryName);
                    Debug.Log($"選択したもの：{choiceEntry.EnglishChoiceEntryName} 正解:{skitChoiceData.Answer} あなたの結果{(choiceEntry.EnglishChoiceEntryName == skitChoiceData.Answer ? "正解" : "不正解")}");
                    LockAndShowAllChoiceButtonsResult();
                    button.ButtonResultImage.gameObject.SetActive(true);
            
                    // 非同期待機: クリック後に再度クリックを待機
                    await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
            
                    awaitEmptyInput.TrySetResult();
                });
                button.ButtonResultImage.gameObject.SetActive(false);
            }
        }
        
        private async UniTask UpdateRestTimeAsync(float time, UniTaskCompletionSource<string> awaitSelect, UniTaskCompletionSource awaitEmptyInput, CancellationToken cancellationToken)
        {
            var decimalPoint = "F1";
            _restTimeText.text = time.ToString(decimalPoint);
            while (time > 0)
            {
                if (awaitSelect.Task.Status == UniTaskStatus.Succeeded || cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                _restTimeText.text = time.ToString(decimalPoint);
                await UniTask.Yield(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    awaitSelect.TrySetResult("");
                    _restTimeText.text = "0.0";
                    LockAndShowAllChoiceButtonsResult();
                    Debug.Log("Time is up");
                    await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: destroyCancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    awaitEmptyInput.TrySetResult();
                }
            }
        }
        
        private void LockAndShowAllChoiceButtonsResult()
        {
            foreach (Transform child in _choiceButtonParent)
            {
                var button = child.GetComponent<SkitChoiceButton>();
                if (button != null)
                {
                    button.ChoiceButton.interactable = false;
                    button.ButtonResultImage.gameObject.SetActive(true);
                }
            }
        }

        private void SetCharacterAndBackground(string backgroundName, SkitTalkCharaData[] talkCharaData)
        {
            if ( _skitResourceLoader.TryGetSpriteByName(backgroundName, out var backGroundSprite))
            {
                _backgroundImage.sprite = backGroundSprite;
            }
            _rightCharaImage.gameObject.SetActive(false);
            _leftCharaImage.gameObject.SetActive(false);
            _middleCharaImage.gameObject.SetActive(false);
            
            if (talkCharaData == null)
            {
                return;
            }
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
                    continue;
                }

                if (!_skitResourceLoader.TryGetSpriteByName(charaData.CharaStateFileName, out var charaSprite)) continue;
                charaImage.sprite = charaSprite;
                charaImage.gameObject.SetActive(true);
            }
        }

        public async UniTask ShowSkit(SkitEntryData skitEntryData, UniTaskCompletionSource skitAwaitCompletionSource, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            SetCharacterAndBackground(skitEntryData.TalkBackground, skitEntryData.TalkCharaData);
            await ShowDialogue(skitEntryData.TalkSpeaker, skitEntryData.JapaneseTalkDialogue, cancellationToken);
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            skitAwaitCompletionSource?.TrySetResult();
        }

        private async UniTask ShowDialogue(string talkerName, string dialogue, CancellationToken cancellationToken)
        {
            _dialoguePanel.SetActive(true);

            // 話者名の表示制御
            if (string.IsNullOrEmpty(talkerName))
            {
                _talkerNamePanel.SetActive(false);
            }
            else
            {
                _talkerNamePanel.SetActive(true);
                _talkerNameText.text = talkerName;
            }

            // ダイアログが空の場合の警告
            if (string.IsNullOrEmpty(dialogue))
            {
                Debug.LogWarning("Dialogue is empty");
                return;
            }

            _dialogueText.text = "";

            var isDialogueComplete = false;

            WaitForSkip().Forget();

            foreach (var c in dialogue)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("ShowDialogue is cancelled");
                    break;
                }
                // スキップ時に全文を即座に表示
                if (isDialogueComplete)
                {
                    _dialogueText.text = dialogue;
                    break;
                }

                _dialogueText.text += c;
                await UniTask.WaitForSeconds(_textSpeed, cancellationToken: cancellationToken);
            }

            async UniTaskVoid WaitForSkip() // スキップ待機
            {
                await UniTask.Yield(cancellationToken);
                while (!isDialogueComplete && !cancellationToken.IsCancellationRequested)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        isDialogueComplete = true;
                        break;
                    }
                    await UniTask.Yield(cancellationToken);
                }
            }
        }


        private void SetActiveFalseAllSkitViewObject()
        {
            _rightCharaImage.gameObject.SetActive(false);
            _leftCharaImage.gameObject.SetActive(false);
            _middleCharaImage.gameObject.SetActive(false);
            _choiceButtonParent.gameObject.SetActive(false);
            _classSelectPanel.SetActive(false);
            _restTimePanel.SetActive(false);
            _statusPanel.SetActive(false);
            _dialoguePanel.SetActive(false);
            _talkerNamePanel.SetActive(false);
        }
        
        
    }
}
