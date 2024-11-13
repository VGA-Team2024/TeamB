using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;
using R3.Triggers;

namespace TeamB.TalkSystem
{
    public class TalkView : MonoBehaviour
    {
        [Header("参照")] 
        [SerializeField] private TalkSystemManager _talkSystemManager = null;
        [Header("共通")] 
        [SerializeField] private Image _backgroundImage = null;
        [SerializeField] private GameObject _loadingPanel = null;
        [Header("クラス選択肢")] [SerializeField] private Transform _choiceButtonParent = null;
        [SerializeField] private List<ChoiceButton> _classSelectButtonPrefab = null;
        [SerializeField] private Sprite _intuitionParameterIcon;
        [SerializeField] private Sprite _readingComprehension;
        [SerializeField] private Sprite _concentration;
        [Header("会話選択肢")]
        [SerializeField] private Transform _talkChoiceButtonParent = null;
        [SerializeField] private List<ChoiceButton> _talkChoiceButtonPrefab = null;
        [SerializeField] private TextMeshProUGUI _talkChoiceTimerText = null;
        [SerializeField] private float _choiceTime = 0;
        [Header("会話")] 
        [SerializeField] private Transform _talkPanel = null;
        [SerializeField] private TextMeshProUGUI _talkText = null;
        [SerializeField] private TextMeshProUGUI _talkerNameText = null;
        [SerializeField] private float _textSpeed = 0.1f;
        [SerializeField] private Transform _characterImageParent = null;
        [SerializeField] private List<Image> _characterImages = null;
        private CancellationTokenSource _talkCancellationTokenSource = new CancellationTokenSource();

        private void Start()
        {
            _talkSystemManager.CurrentTalkEntryData
                .Where(_ => _talkSystemManager.CurrentTalkState == TalkSystemManager.TalkState.Talk)
                .Subscribe(_ =>
                {
                    _characterImageParent.gameObject.SetActive(true);
                    _choiceButtonParent.gameObject.SetActive(false);
                    _talkPanel.gameObject.SetActive(true);
                    _talkChoiceButtonParent.gameObject.SetActive(false);
                    ShowTalkData();
                    ShowTalkCharacter();
                }).AddTo(_talkSystemManager);

            _talkSystemManager.CurrentClassSelectData
                .Where(_ => _talkSystemManager.CurrentTalkState == TalkSystemManager.TalkState.ClassSelect)
                .Subscribe(_ =>
                {
                    _characterImageParent.gameObject.SetActive(false);
                    _choiceButtonParent.gameObject.SetActive(true);
                    _talkPanel.gameObject.SetActive(false);
                    _talkChoiceButtonParent.gameObject.SetActive(false);
                    SetClassSelectData();
                }).AddTo(_talkSystemManager);

            _talkSystemManager.CurrentClassChoiceData
                .Where(_ => _talkSystemManager.CurrentTalkState == TalkSystemManager.TalkState.Choice)
                .Subscribe(_ =>
                {
                    _characterImageParent.gameObject.SetActive(true);
                    _choiceButtonParent.gameObject.SetActive(false);
                    _talkPanel.gameObject.SetActive(true);
                    _talkChoiceButtonParent.gameObject.SetActive(true);
                    SetTalkChoiceData();
                    ShowTalkCharacter();
                    CountDown(_talkSystemManager.CurrentClassChoiceData.CurrentValue.ChoiceTime);
                }).AddTo(_talkSystemManager);

            this.UpdateAsObservable()
                .Where(_ => _talkSystemManager.CurrentTalkState == TalkSystemManager.TalkState.Talk)
                .Subscribe(_ =>
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _talkSystemManager.GoNextTalk = true;
                    }
                }).AddTo(_talkSystemManager);

            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    _loadingPanel.SetActive(_talkSystemManager.CurrentTalkState == TalkSystemManager.TalkState.Init);
                }).AddTo(_talkSystemManager);
        }

        private void ShowTalkCharacter()
        {
            _characterImages.ForEach(image => image.gameObject.SetActive(false));
            foreach (var talkCharaData in _talkSystemManager.CurrentTalkEntryData.CurrentValue.TalkCharaData)
            {
                var charaName = talkCharaData.CharaName;
                var stateName = talkCharaData.CharaStateName;
                var sprite = _talkSystemManager.ConstantTalkData.CharacterDataList.FirstOrDefault(x => x.CharacterID == charaName)
                    ?.TalkDataList.FirstOrDefault(x => x.StateName == stateName);
                var pos = talkCharaData.StandingPosition;
                switch (pos)
                {
                    case StandingPosition.Left:
                        if (sprite != null)
                        {
                            _characterImages[0].gameObject.SetActive(true);
                            _characterImages[0].sprite = sprite.CharaSprite;
                        }
                        break;
                    case StandingPosition.Middle:
                        if (sprite != null)
                        {
                            _characterImages[1].gameObject.SetActive(true);
                            _characterImages[1].sprite = sprite.CharaSprite;
                        }
                        break;
                    case StandingPosition.Right:
                        if (sprite != null)
                        {
                            _characterImages[2].gameObject.SetActive(true);
                            _characterImages[2].sprite = sprite.CharaSprite;
                        }
                        break;
                }
            }
        }

        private void SetTalkChoiceData()
        {
            for (var i = 0; i < _talkChoiceButtonPrefab.Count; i++)
            {
                if (i >= _talkSystemManager.CurrentClassChoiceData.CurrentValue.ChoiceEntries.Length)
                {
                    _talkChoiceButtonPrefab[i].gameObject.SetActive(false);
                    continue;
                }

                _talkerNameText.text = _talkSystemManager.CurrentTalkEntryData.CurrentValue.TalkSpeaker;
                _talkChoiceButtonPrefab[i].SetText(_talkSystemManager.CurrentClassChoiceData.CurrentValue.ChoiceEntries[i]
                    .JapaneseChoiceEntryName);
                var index = i;
                _talkChoiceButtonPrefab[i].Button.OnClickAsObservable().Take(1)
                    .Subscribe(_ =>
                        _talkSystemManager.UniTaskCompletionSource.TrySetResult(((index + 1).ToString(), _choiceTime.ToString(CultureInfo.CurrentCulture))))
                    .AddTo(_talkSystemManager);
                SetDialogueText(_talkSystemManager.CurrentTalkEntryData.CurrentValue.JapaneseTalkDialogue).Forget();
            }
        }

        private void CountDown(float time)
        {
            _choiceTime = time;
            Observable.Interval(TimeSpan.FromSeconds(0.1f)) 
                .TakeWhile(_ => _choiceTime >= 0)
                .Subscribe(_ =>
                {
                    _choiceTime -= 0.1f;
                    _talkChoiceTimerText.text = $"残り: {_choiceTime:F1}秒";
                })
                .AddTo(this);
        }

        private void SetClassSelectData()
        {
            if (_talkSystemManager.CurrentClassSelectData == null)
            {
                Debug.LogError("ClassSelectData is null");
                return;
            }

            for (var i = 0; i < _classSelectButtonPrefab.Count; i++)
            {
                if (_talkSystemManager.CurrentClassSelectData.CurrentValue.ClassChoices == null)
                {
                    Debug.LogError("ClassChoices is null");
                    return;
                }
                if (i >= _talkSystemManager.CurrentClassSelectData.CurrentValue.ClassChoices.Length)
                {
                    _classSelectButtonPrefab[i].gameObject.SetActive(false);
                    Debug.Log("ClassChoices is null");
                    continue;
                }

                _classSelectButtonPrefab[i].SetText(_talkSystemManager.CurrentClassSelectData.CurrentValue.ClassChoices[i]
                    .JapaneseChoiceName);
                _classSelectButtonPrefab[i].SetIcon(GetRewardIcon(_talkSystemManager.CurrentClassSelectData.CurrentValue
                    .ClassChoices[i].TalkReward));
                var index = i;
                // ここで授業の選択をした際の、開く会話のIDと報酬をUniTaskCompletionSourceにセットしている
                _classSelectButtonPrefab[i].Button.OnClickAsObservable().Take(1)
                    .Subscribe(_ => _talkSystemManager.UniTaskCompletionSource
                        .TrySetResult((
                            _talkSystemManager.CurrentClassSelectData.CurrentValue.ClassChoices[index].TalkDataId,
                            _talkSystemManager.CurrentClassSelectData.CurrentValue.ClassChoices[index].TalkReward
                                .ToString())))
                    .AddTo(_talkSystemManager);
            }
        }

        private void ShowTalkData()
        {
            if (_talkSystemManager.CurrentTalkEntryData == null)
            {
                Debug.LogError("TalkEntryData is null");
                return;
            }

            _talkerNameText.text = _talkSystemManager.CurrentTalkEntryData.CurrentValue.TalkSpeaker;
            var dialogue = _talkSystemManager.CurrentTalkEntryData.CurrentValue.JapaneseTalkDialogue;
            SetDialogueText(dialogue).Forget();
        }

        private async UniTask SetDialogueText(string dialogue)
        {
            _talkCancellationTokenSource.Cancel();
            _talkCancellationTokenSource = new CancellationTokenSource();
            _talkText.text = "";
            foreach (var c in dialogue)
            {
                _talkText.text += c;
                await UniTask.Delay(TimeSpan.FromSeconds(_textSpeed),
                    cancellationToken: _talkCancellationTokenSource.Token);
            }
        }

        private Sprite GetRewardIcon(RewardType reward)
        {
            return reward switch
            {
                RewardType.Intuition => _intuitionParameterIcon,
                RewardType.ReadingComprehension => _readingComprehension,
                RewardType.Concentration => _concentration,
                _ => null
            };
        }
    }
}