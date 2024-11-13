using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace TeamB.TalkSystem
{
    public class TalkSystemManager : MonoBehaviour
    {
        public enum LoadType
        {
            Local,
            Remote,
        }
            
        [SerializeField] private string _talkDataId;
        [SerializeField] private TalkState _currentTalkState = TalkState.Init;
        [SerializeField] private LoadType _loadType = LoadType.Local;
        private const string TalkChoiceKey = "[Choice]";
        private const string Pattern = @"\[(.*?)\]";
        private ITalkDataLoader _talkDataLoader = null;

        public UniTaskCompletionSource<(string choiceId, string result)> UniTaskCompletionSource { get; private set; } = null;

        #region UIで表示するための外だしデータ
        public ConstantTalkData ConstantTalkData { get; private set; } = null;

        public bool GoNextTalk { get; set; } = false;
        public TalkState CurrentTalkState => _currentTalkState;
        private ReactiveProperty<ClassTalkEntryData> _currentTalkEntryData = new();
        public ReadOnlyReactiveProperty<ClassTalkEntryData> CurrentTalkEntryData => _currentTalkEntryData;
        private ReactiveProperty<ClassSelectData> _currentClassSelectData = new();
        public ReadOnlyReactiveProperty<ClassSelectData> CurrentClassSelectData => _currentClassSelectData;
        private ReactiveProperty<ChoiceData> _currentClassChoiceData = new();
        public ReadOnlyReactiveProperty<ChoiceData> CurrentClassChoiceData => _currentClassChoiceData;
        public RewardType CurrentRewardType { get; private set; } = RewardType.Intuition;

        #endregion

        public enum TalkState
        {
            Init,
            ClassSelect,
            Talk,
            Choice,
        }

        private void Start()
        {
            DoTalkSequence().Forget();
        }

        private async UniTask DoTalkSequence()
        {
            _currentTalkState = TalkState.Init;
            await InitTalkDataLoader();
            GoNextTalk = false;
            //選択肢のデータをセット
            _currentTalkState = TalkState.ClassSelect;
            if (_talkDataLoader.TryGetClassSelectData(_talkDataId, out var currentClassSelectData))
            {
                _currentClassSelectData.Value = currentClassSelectData;
            }
            else
            {
                Debug.LogError("ClassSelectData is not found.");
                return;
            }

            //クラス選択
            UniTaskCompletionSource = new UniTaskCompletionSource<(string choiceId, string result)>();
            var choice = await UniTaskCompletionSource.Task;
            CurrentRewardType = (RewardType)Enum.Parse(typeof(RewardType), choice.result);
            //授業選択肢のデータのロード
            _currentTalkState = TalkState.Talk;
            if (!_talkDataLoader.TryGetTalkData(choice.choiceId, out var talkData))
            {
                Debug.LogError("TalkData is not found.");
                return;
            }
            foreach (var talkEntry in talkData.TalkData)
            {
                //会話データをセット&会話データを表示
                if (talkEntry.JapaneseTalkDialogue.Contains(TalkChoiceKey))
                {
                    _currentTalkState = TalkState.Choice;
                    var normText = talkEntry.JapaneseTalkDialogue.Replace(TalkChoiceKey, "");
                    var choiceData = GetChoiceId(normText);
                    talkEntry.JapaneseTalkDialogue = choiceData.dialog;
                    _currentTalkEntryData.Value = talkEntry;
                    if (_talkDataLoader.TryGetChoiceData(choiceData.choiceId, out var classChoice))
                    {
                        _currentClassChoiceData.Value = classChoice;
                        UniTaskCompletionSource = new UniTaskCompletionSource<(string choiceId, string result)>();
                        //ここで選択肢のデータを元に加算処理をする
                        await UniTask.WhenAny(UniTaskCompletionSource.Task, UniTask.Delay(TimeSpan.FromSeconds(_currentClassChoiceData.Value.ChoiceTime)));
                    }
                    else
                    {
                        Debug.LogError("ChoiceData is not found.");
                        return;
                    }
                }
                else
                {
                    _currentTalkState = TalkState.Talk;
                    _currentTalkEntryData.Value = talkEntry;
                    await UniTask.WaitUntil(() => GoNextTalk);
                    //会話データを進める
                    GoNextTalk = false;
                }
            }
            //会話終了
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SceneLoader.LoadScene("Exam");
            GameStatics.PrevGameState = GameState.Exam;
        }

        private async UniTask InitTalkDataLoader()
        {
            if (_loadType == LoadType.Remote)
            {
                _talkDataLoader = new RemoteTalkDataLoader();
            }
            else
            {
                _talkDataLoader = new LocalTalkDataLoader();
            }
            await _talkDataLoader.LoadClassSelectData();
            await _talkDataLoader.LoadTalkData();
            await _talkDataLoader.LoadChoiceData();
            var loadConstantTalkDataHandle = Addressables.LoadAssetAsync<ConstantTalkData>("TalkData");
            ConstantTalkData = await loadConstantTalkDataHandle.Task;
            Debug.Log(ConstantTalkData.CharacterDataList.Count);
        }

        private (string choiceId, string dialog) GetChoiceId(string normDialogue)
        {
            var result = (choiceId: "", dialog: "");
            var match = Regex.Match(normDialogue, Pattern);
            if (match.Success)
            {
                // マッチしたグループから "Choice001" を抽出
                var id = match.Groups[1].Value;
                var dialog = normDialogue.Replace(match.Value, "");
                result = (id, dialog);
            }
            else
            {
                Debug.LogError("マッチするデータがありません");
            }

            return result;
        }
    }

    #region 授業選択肢

    /// <summary>
    /// 授業選択肢をまとめたデータ
    /// </summary>
    [Serializable]
    public class ClassSelectData
    {
        public string ClassSelectId;
        public string BackgroundImageName;
        public ClassChoiceData[] ClassChoices;
    }

    /// <summary>
    /// それぞれの授業選択肢のデータ
    /// </summary>
    [Serializable]
    public class ClassChoiceData
    {
        public string ChoiceName;
        public string JapaneseChoiceName;
        public string TalkDataId;
        public RewardType TalkReward;
    }

    [Serializable]
    public enum RewardType
    {
        None,
        Intuition, //直観力
        ReadingComprehension, //読解力
        Concentration, //集中力
    }

    #endregion

    #region 会話データ

    [Serializable]
    public class ClassTalkData
    {
        public string TalkDataId;
        public ClassTalkEntryData[] TalkData;
    }

    [Serializable]
    public class ClassTalkEntryData
    {
        public ClassTalkCharaData[] TalkCharaData; //キャラの立ち位置などをまとめたデータ
        public string TalkSpeaker;
        public string TalkBackground;
        public string JapaneseTalkDialogue;
        public string EnglishTalkDialogue;
    }

    public enum StandingPosition
    {
        Left,
        Middle,
        Right,
    }

    [Serializable]
    public class ClassTalkCharaData
    {
        public string CharaName;
        public StandingPosition StandingPosition;
        public string CharaStateName;
    }

    #endregion

    #region 選択肢データ

    [Serializable]
    public class ChoiceData
    {
        public string ChoiceId;
        public float ChoiceTime;
        public ChoiceEntry[] ChoiceEntries;
        public int AnswerIndex;
    }

    [Serializable]
    public class ChoiceEntry
    {
        public string ChoiceEntryId;
        public string EnglishChoiceEntryName;
        public string JapaneseChoiceEntryName;
    }

    #endregion
}