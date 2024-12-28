using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using TeamB.GameSystem.Statics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitView : MonoBehaviour
    {
        [Header("操作系")]
        [SerializeField] private SkitSceneButtonBase _skipButton;
        [SerializeField] private SkitSceneButtonBase _autoButton;
        private const float AutoDelaySpeed = 2f;
        [SerializeField] private SkitSceneButtonBase _backLogButton;
        [SerializeField] private SkitLogViewer _backlogView;
        [SerializeField] private RectTransform _backLogTextParent;
        [Header("会話表示関連")] 
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private TMP_Text _talkerNameText;
        [SerializeField] private GameObject _talkerNamePanel;
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private SkitEndMaker _skitEndMaker;
        [SerializeField, Range(0, 0.2f)] private float _textSpeed = 0.03f;
        [Header("背景・キャラ表示関連")] 
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _leftCharaImage;
        [SerializeField] private Image _rightCharaImage;
        [SerializeField] private Image _middleCharaImage;
        [SerializeField, Range(0, 1f)] private float _charaFadeTime = 0.5f;
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
        [SerializeField] private Image _intuitionImage;
        [SerializeField] private Image _readingComprehensionImage;
        [SerializeField] private Image _concentrationImage;
        [SerializeField] private Image _statusUpImage;
        [Header("チュートリアル用")]
        [SerializeField] private GameObject _tutorialPanelAboutGame;
        [SerializeField] private GameObject _tutorialPanelAboutClassSelect;
        [SerializeField] private GameObject _tutorialPanelAboutSkitChoice;
        [SerializeField] private GameObject _tutorialPanelAboutSkitResult;
        [Header("その他")]
        [SerializeField] private SkitViewFade _skitFadeView;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private InputType _inputType;
        private bool _isFirstSkitContextExecuted;
        private SkitResourceLoader _skitResourceLoader;
        private const float ClassBellTime = 3f;
        private const float AfterClassSelectTime = 5f;
        
        private enum InputType
        {
            Tap,
            Auto,
            Skip
        }

        public void InitializeSkitView(SkitResourceLoader skitResourceLoader)
        {
            _skitResourceLoader = skitResourceLoader;
            var key = (int)GameStatics.NurturingCharacterType;
            if (GameStatics.Characters.ContainsKey(key))
            {
                Observable.EveryValueChanged(GameStatics.Characters[key], value => value.MagicATK)
                    .Subscribe(x => UpdateStatus(x, _intuitionText, _intuitionImage.rectTransform)).AddTo(this);
                Observable.EveryValueChanged(GameStatics.Characters[key], value => value.ChantingSpeed)
                    .Subscribe(x => UpdateStatus(x, _readingComprehensionText, _readingComprehensionImage.rectTransform)).AddTo(this);
                Observable.EveryValueChanged(GameStatics.Characters[key], value => value.HitRate)
                    .Subscribe(x => UpdateStatus(x, _concentrationText, _concentrationImage.rectTransform)).AddTo(this);
            }
            _statusUpImage.gameObject.SetActive(false);
            _backLogButton.OnClick += () => _backlogView.SetActivePanel(true);
            _skipButton.OnClick += SetSkip;
            _autoButton.OnClick += SetAuto;
        }
        
        private void SetSkip()
        {
            CRIAudioManager.VOICE.Stop();
            _inputType = InputType.Skip;
            _skipButton.ShowIsActivated(_inputType == InputType.Skip);
            _autoButton.ShowIsActivated(false);
        }

        private void SetAuto()
        {
            _inputType = _inputType == InputType.Auto ? InputType.Tap : InputType.Auto;
            _autoButton.ShowIsActivated(_inputType == InputType.Auto);
            _skipButton.ShowIsActivated(false);
        }

        private void SetTap()
        {
            _inputType = InputType.Tap;
            _autoButton.ShowIsActivated(false);
            _skipButton.ShowIsActivated(false);
        }

        private void UpdateStatus(float currentValue, TMP_Text statusText, RectTransform goalObject)
        {
            _statusUpImage.rectTransform.anchoredPosition = goalObject.anchoredPosition  - new Vector2(0, 10);
            MoveStatusUp();
            statusText.text = $"{currentValue:F1}";
        }

        private void MoveStatusUp()
        {
            _statusUpImage.gameObject.SetActive(true);
            _statusUpImage.color = new Color(1, 1, 1, 1);
            _statusUpImage.DOFade(0, 1.0f).SetEase(Ease.Linear).SetLink(gameObject);
            _statusUpImage.rectTransform.DOAnchorPosY(0, 1.0f).SetEase(Ease.Linear).SetLink(gameObject);
        }
        
        private async UniTask GetEmptyInput(CancellationToken cancellationToken)
        {
            _skitEndMaker.gameObject.SetActive(true);
            switch (_inputType)
            {
                case InputType.Tap:
                    while (true)
                    {
                        if (_inputType == InputType.Skip) return;
                        if (_inputType == InputType.Auto)
                        {
                            GetEmptyInput(cancellationToken).Forget();
                            break;
                        }
                        if (!_backlogView.IsLogActive && GetMouseButtonDown())
                        {
                            break;
                        }
                        await UniTask.Yield(cancellationToken: cancellationToken);
                    }
                    break;
                case InputType.Auto:
                    var elapsedTime = 0f;
                    while (elapsedTime < AutoDelaySpeed)
                    {
                        if (_inputType == InputType.Skip) return;
                        if (_backlogView.IsLogActive)
                        {
                            await UniTask.Yield(cancellationToken: cancellationToken);
                            continue;
                        }
                        if (_inputType == InputType.Tap)
                        {
                            break;
                        }
                        await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                        elapsedTime += Time.deltaTime;
                    }
                    break;
                case InputType.Skip:
                    _skitEndMaker.gameObject.SetActive(false);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _skitEndMaker.gameObject.SetActive(false);
        }

        private bool GetMouseButtonDown()
        {
            return Input.GetMouseButtonDown(0) && !IsPointerOverButton();
        }
        
        private bool IsPointerOverButton()
        {
            var pointerData = new PointerEventData(_eventSystem)
            {
                position = Input.mousePosition
            };
            var results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(pointerData, results);
            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Button>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        public async UniTask ShowTutorialAboutGame(NormalTutorialData tutorialData, UniTaskCompletionSource emptyInput,
            CancellationToken cancellationToken)
        {
            SetTap();
            SetActiveFalseAllSkitViewObject();
            await SetCharacterAndBackground(tutorialData.BackgroundImageName, null, cancellationToken);
            _tutorialPanelAboutGame.SetActive(true);
            await GetEmptyInput(cancellationToken);
            emptyInput.TrySetResult();
            _tutorialPanelAboutGame?.SetActive(false);
        }

        public async UniTask ShowTutorialAboutClassSelect(TutorialClassSelectData tutorialData,
            UniTaskCompletionSource<string> awaitSelect, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            await SetCharacterAndBackground(tutorialData.BackgroundImageName, null, cancellationToken);
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
                button.OnClick += () =>
                {
                    awaitSelect.TrySetResult(classSelectEntry.TalkDataId);
                    _classSelectPanel.SetActive(false);
                    _tutorialPanelAboutClassSelect?.SetActive(false);
                };
            }
        }

        public async UniTask ShowTutorialAboutSkitChoice(TutorialChoiceData tutorialChoiceData,
            UniTaskCompletionSource<string> awaitSelect, CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            await SetCharacterAndBackground(tutorialChoiceData.TalkBackground, null, cancellationToken);
            await ShowDialogue(tutorialChoiceData.TalkSpeaker, tutorialChoiceData.JapaneseTalkDialogue,
                cancellationToken);
            _tutorialPanelAboutSkitChoice.SetActive(true);
            _choiceButtonParent.gameObject.SetActive(true);
            _statusPanel.SetActive(true);
            _restTimePanel.SetActive(true);
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Operation was cancelled.");
                return;
            }

            foreach (Transform child in _choiceButtonParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var choiceEntry in tutorialChoiceData.ChoiceEntries)
            {
                var button = Instantiate(_choiceButtonPrefab, _choiceButtonParent);
                var resultSprite = _missChoiceSprite;
                if (string.Equals(choiceEntry.EnglishChoiceEntryName, tutorialChoiceData.Answer))
                {
                    resultSprite = _correctChoiceSprite;
                }

                button.InitializeSkitChoiceButton(choiceEntry.JapaneseChoiceEntryName, resultSprite);

                button.OnClick += () =>
                {
                    awaitSelect.TrySetResult(tutorialChoiceData.Answer);
                    LockAndShowAllChoiceButtonsResult();
                    button.ButtonResultImage.gameObject.SetActive(true);
                    CRIAudioManager.SE.Play(SkitSoundHelper.SeSheetName, SkitSoundHelper.Correct);
                    CRIAudioManager.SE.Play(SkitSoundHelper.SeSheetName, SkitSoundHelper.ParameterUp);
                };
                button.ButtonResultImage.gameObject.SetActive(false);
            }
        }

        public async UniTask ShowTutorialAboutSkitResult(NormalTutorialData tutorialData,
            UniTaskCompletionSource awaitForEmptyInput, CancellationToken cancellationToken)
        {
            await SetCharacterAndBackground(tutorialData.BackgroundImageName, null, cancellationToken);
            UpdateStatus(100, _intuitionText, _intuitionImage.rectTransform);
            _tutorialPanelAboutSkitChoice.SetActive(false);
            _tutorialPanelAboutSkitResult.SetActive(true);
            _statusPanel.SetActive(true);
            await GetEmptyInput(cancellationToken);
            awaitForEmptyInput.TrySetResult();
            _tutorialPanelAboutSkitResult?.SetActive(false);
        }

        public async UniTask ShowClassSelect(ClassSelectData classSelectData,
            UniTaskCompletionSource<string> awaitSelect, CancellationToken cancellationToken)
        {
             
            if (!_skitFadeView.IsFading) await _skitFadeView.FadeInAsync(cancellationToken);
            SetActiveFalseAllSkitViewObject();
            await SetCharacterAndBackground(classSelectData.BackgroundImageName, null, cancellationToken);
            if (_skitFadeView.IsFading) await _skitFadeView.FadeOutAsync(cancellationToken);
            CRIAudioManager.SE.Play(SkitSoundHelper.SeSheetName, SkitSoundHelper.ClassBell);
            await UniTask.WaitForSeconds(ClassBellTime, cancellationToken: cancellationToken);
            await ShowDialogue(classSelectData.TalkerName, classSelectData.Dialogue, cancellationToken);
            CRIAudioManager.SE.Play(SkitSoundHelper.LianSheetName, SkitSoundHelper.VoiceClassSelect);
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Operation was cancelled.");
                return;
            }

            _classSelectPanel.SetActive(true);
            _restDayText.text = $"残り{classSelectData.RemainDay}日";
            foreach (Transform child in _classSelectButtonParent)
            {
                Destroy(child.gameObject);
            }

            var buttons = new List<ClassSelectButton>();
            foreach (var classSelectEntry in classSelectData.ClassChoices)
            {
                var button = Instantiate(_classSelectButtonPrefab, _classSelectButtonParent);
                buttons.Add(button);
                button.InitializeClassSelectButton(classSelectEntry.JapaneseChoiceName, classSelectEntry.TalkReward);
                button.OnClick += async () =>
                {
                    CRIAudioManager.VOICE.Play(SkitSoundHelper.LianSheetName, SkitSoundHelper.VoiceAfterClassSelect);
                    buttons.ForEach(b => b.LockButton(true));
                    await UniTask.WaitForSeconds(AfterClassSelectTime, cancellationToken: cancellationToken);
                    awaitSelect.TrySetResult(classSelectEntry.TalkDataId);
                    _classSelectPanel.SetActive(false);
                };
            }
        }

        public async UniTask ShowSkitChoice(SkitChoiceData skitChoiceData, UniTaskCompletionSource<string> awaitChoice,
            UniTaskCompletionSource awaitEmptyInput, float time, CancellationToken cancellationToken)
        {
            _inputType = InputType.Tap;
            _autoButton.ShowIsActivated(false);
            _autoButton.LockButton(true);
            _skipButton.LockButton(true);
            SetActiveFalseAllSkitViewObject();
            await SetCharacterAndBackground(skitChoiceData.TalkBackground, skitChoiceData.TalkCharaData, cancellationToken);
            await ShowDialogue(skitChoiceData.TalkSpeaker, skitChoiceData.JapaneseTalkDialogue, cancellationToken);
            //CRIAudioManager.VOICE.Play(SkitSoundKey.LianSheetName, SkitSoundKey.VoiceChoiceSuggestion);
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Operation was cancelled.");
                return;
            }

            _statusPanel.SetActive(true); //ステータス表示
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

                button.InitializeSkitChoiceButton(choiceEntry.JapaneseChoiceEntryName, resultSprite);

                button.OnClick += async () =>
                {
                    awaitChoice.TrySetResult(choiceEntry.EnglishChoiceEntryName);
                    LockAndShowAllChoiceButtonsResult();
                    button.ButtonResultImage.gameObject.SetActive(true);
                    _backlogView.SetUserAnswerLog(choiceEntry.JapaneseChoiceEntryName);
                    // 非同期待機: クリック後に再度クリックを待機
                    await GetEmptyInput(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    awaitEmptyInput.TrySetResult();
                    _autoButton.LockButton(false);
                    _skipButton.LockButton(false);
                };
                button.ButtonResultImage.gameObject.SetActive(false);
            }
        }

        private async UniTask UpdateRestTimeAsync(float time, UniTaskCompletionSource<string> awaitSelect,
            UniTaskCompletionSource awaitEmptyInput, CancellationToken cancellationToken)
        {
            var decimalPoint = "F1";
            _restTimeText.text = time.ToString(decimalPoint);
            while (time > 0)
            {
                if (_backlogView.IsLogActive)
                {
                    await UniTask.Yield(cancellationToken);
                    continue;
                }

                if (awaitSelect.Task.Status == UniTaskStatus.Succeeded || cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _restTimeText.text = $"残り{time.ToString(decimalPoint)}秒";
                await UniTask.Yield(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    awaitSelect.TrySetResult("");
                    _restTimeText.text = "残り0.0秒";
                    LockAndShowAllChoiceButtonsResult();
                    Debug.Log("Time is up");
                    await UniTask.WaitUntil(GetMouseButtonDown,
                        cancellationToken: destroyCancellationToken);
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
                    button.LockButton(true);
                    button.ButtonResultImage.gameObject.SetActive(true);
                }
            }
        }

        private async UniTask SetCharacterAndBackground(string backgroundName, SkitTalkCharaData[] talkCharaData, CancellationToken cancellationToken)
        {
            if (_skitResourceLoader.TryGetSpriteByName(backgroundName, out var backGroundSprite))
            {
                _backgroundImage.sprite = backGroundSprite;
            }

            _rightCharaImage.gameObject.SetActive(false);
            _leftCharaImage.gameObject.SetActive(false);
            _middleCharaImage.gameObject.SetActive(false);

            if (talkCharaData == null)
            {
                await FirstFade(cancellationToken);
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

                if (!_skitResourceLoader.TryGetSpriteByName(charaData.CharaStateFileName, out var charaSprite))
                    continue;
                charaImage.gameObject.SetActive(true);
                var isSameSprite = charaImage.sprite == charaSprite;
                charaImage.sprite = charaSprite;
                if (!isSameSprite) ShowFadeChara(charaImage);
            }

            //次表示する際にFadeさせるためにnullを入れる
            if (!_leftCharaImage.gameObject.activeSelf) _leftCharaImage.sprite = null;
            if (!_rightCharaImage.gameObject.activeSelf) _rightCharaImage.sprite = null;
            if (!_middleCharaImage.gameObject.activeSelf) _middleCharaImage.sprite = null;
            await FirstFade(cancellationToken);
            
            async UniTask FirstFade(CancellationToken localCancellationToken)
            {
                if (!_isFirstSkitContextExecuted)
                {
                    await _skitFadeView.FadeOutAsync(localCancellationToken);
                    _isFirstSkitContextExecuted = true;
                }
            }
        }

        

        private void ShowFadeChara(Image charaImage)
        {
            charaImage.color = new Color(1, 1, 1, 0);
            charaImage.DOFade(1, _charaFadeTime).SetEase(Ease.Linear).SetLink(gameObject);
        }

        public async UniTask ShowSkit(SkitEntryData skitEntryData, UniTaskCompletionSource skitAwaitCompletionSource,
            CancellationToken cancellationToken)
        {
            SetActiveFalseAllSkitViewObject();
            await SetCharacterAndBackground(skitEntryData.TalkBackground, skitEntryData.TalkCharaData,
                cancellationToken);
            if (CRIAudioManager.VOICE.IsPlaying) CRIAudioManager.VOICE.Stop();
            if (!string.IsNullOrEmpty(skitEntryData.VoiceFileName) && _inputType != InputType.Skip) CRIAudioManager.VOICE.Play(SkitSoundHelper.GetVoiceCueSheetName(skitEntryData.VoiceFileName), skitEntryData.VoiceFileName);
            await ShowDialogue(skitEntryData.TalkSpeaker, skitEntryData.JapaneseTalkDialogue,  cancellationToken, skitEntryData.VoiceFileName);
            await UniTask.WaitUntil(() => !CRIAudioManager.VOICE.IsPlaying, cancellationToken: cancellationToken);
            await GetEmptyInput(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            skitAwaitCompletionSource?.TrySetResult();
        }

        private async UniTask ShowDialogue(string talkerName, string dialogue ,CancellationToken cancellationToken, string voiceFileName = "")
        {
            _dialoguePanel.SetActive(true);
            //CRIAudioManager.VOICE.Play("Voice", voiceFileName);

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
                if (isDialogueComplete || _inputType == InputType.Skip)
                {
                    _dialogueText.text = dialogue;
                    break;
                }

                _dialogueText.text += c;
                CRIAudioManager.SE.Play(SkitSoundHelper.SeSheetName, SkitSoundHelper.TextFeed);
                await UniTask.WaitForSeconds(_textSpeed, cancellationToken: cancellationToken);
            }

            async UniTaskVoid WaitForSkip() // スキップ待機
            {
                await UniTask.Yield(cancellationToken);
                while (!isDialogueComplete && !cancellationToken.IsCancellationRequested)
                {
                    if (GetMouseButtonDown())
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
            _tutorialPanelAboutGame?.SetActive(false);
            _tutorialPanelAboutClassSelect?.SetActive(false);
            _tutorialPanelAboutSkitChoice?.SetActive(false);
            _tutorialPanelAboutSkitResult?.SetActive(false);
        }
    }
}
