using System;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーンで用いられるデータのインターフェース。
    /// 各データをインターフェースでまとめることで、どの部分からでも会話シーンを開始できるようにした。
    /// またSkitContextで共通のデータとして渡すことができるのでSkitContext内にそれぞれのクラスの変数を持たせる必要がなくなる。
    /// </summary>
    public interface ISkitSceneData
    {
        public string Id { get; }
    }
    
    #region 授業選択肢

    /// <summary>
    /// 授業選択肢をまとめたデータ
    /// </summary>
    [Serializable]
    public class ClassSelectData : ISkitSceneData
    {
        private string _classSelectId;
        private string _classSelectTalkerName;
        private string _classSelectBackgroundImageName;
        private string _classSelectDialogue;
        public ClassChoiceData[] ClassChoices;
        public string Id => _classSelectId;
        public string TalkerName => _classSelectTalkerName;
        public string Dialogue => _classSelectDialogue;
        public string BackgroundImageName => _classSelectBackgroundImageName;
        
        public ClassSelectData(string classSelectId, string classSelectTalkerName, string classSelectBackgroundImageName, string classSelectDialogue, ClassChoiceData[] classChoices)
        {
            _classSelectId = classSelectId;
            _classSelectTalkerName = classSelectTalkerName;
            _classSelectBackgroundImageName = classSelectBackgroundImageName;
            _classSelectDialogue = classSelectDialogue;
            ClassChoices = classChoices;
        }
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
    /// <summary>
    /// 各会話をまとめたデータ、CurrentTalkDataIndexで現在の会話を指定して取得する
    /// </summary>
    [Serializable]
    public class SkitData : ISkitSceneData
    {
        private string _skitDataId;
        private SkitEntryData[] _skitEntryData;
        public string Id => _skitDataId;
        public SkitEntryData[] SkitEntryData => _skitEntryData;
        
        public SkitData(string skitDataId, SkitEntryData[] skitEntryData)
        {
            _skitDataId = skitDataId;
            _skitEntryData = skitEntryData;
        }
    }

    [Serializable]
    public class SkitEntryData
    {
        public SkitTalkCharaData[] TalkCharaData; //キャラの立ち位置などをまとめたデータ
        public string TalkSpeaker;
        public string TalkBackground;
        public string JapaneseTalkDialogue;
        public string EnglishTalkDialogue;
    }

    public enum StandingPosition
    {
        None,
        Left,
        Middle,
        Right,
    }

    [Serializable]
    public class SkitTalkCharaData
    {
        public string CharaName;
        public StandingPosition StandingPosition = StandingPosition.None;
        public string CharaStateFileName;
    }
    #endregion
    
    #region 選択肢データ
    
    /// <summary>
    /// 会話内の各選択肢のデータ
    /// </summary>
    [Serializable]
    public class SkitChoiceData : ISkitSceneData
    {
        private string _choiceId;
        private string _talkerName;
        private string _backgroundImageName;
        private string _dialogue;
        private float _choiceTime;
        private string _answer;
        private ChoiceEntry[] _choiceEntries;
        public string Id => _choiceId;
        public string TalkerName => _talkerName;
        public string Dialogue => _dialogue;
        public string BackgroundImageName => _backgroundImageName;
        public string Answer => _answer;
        public float ChoiceTime => _choiceTime;
        public ChoiceEntry[] ChoiceEntries => _choiceEntries;
        
        public SkitChoiceData(string choiceId, string talkerName, string backgroundImageName, string dialogue, float choiceTime, string answer, ChoiceEntry[] choiceEntries)
        {
            _choiceId = choiceId;
            _talkerName = talkerName;
            _backgroundImageName = backgroundImageName;
            _dialogue = dialogue;
            _choiceTime = choiceTime;
            _answer = answer;
            _choiceEntries = choiceEntries;
        }
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

