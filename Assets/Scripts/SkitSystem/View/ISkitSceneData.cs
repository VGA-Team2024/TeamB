using System;
using Newtonsoft.Json;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーンで用いられるデータのインターフェース。
    /// 各データをインターフェースでまとめることで、どの部分からでも会話シーンを開始できるようにした。
    /// またSkitContextで共通のデータとして渡せるのでSkitContext内にそれぞれのクラスの変数を持たせる必要がなくなる。
    /// </summary>
    public interface ISkitSceneData
    {
        public string Id { get; }
        public string Flag { get; }
    }

    #region 授業選択肢

    /// <summary>
    /// 授業選択肢をまとめたデータ
    /// </summary>
    [Serializable]
    public class ClassSelectData : ISkitSceneData
    {
        public ClassChoiceData[] ClassChoices;
        public string Id { get; }
        public string Flag { get; }
        public int RemainDay { get; }

        public string TalkerName { get; }

        public string Dialogue { get; }

        public string BackgroundImageName { get; }

        public ClassSelectData(string classSelectId, string flag, int remainDay, string classSelectTalkerName,
            string classSelectBackgroundImageName, string classSelectDialogue, ClassChoiceData[] classChoices)
        {
            Id = classSelectId;
            Flag = flag;
            RemainDay = remainDay;
            TalkerName = classSelectTalkerName;
            BackgroundImageName = classSelectBackgroundImageName;
            Dialogue = classSelectDialogue;
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
    public class SkitData : ISkitSceneData
    {
        public string Id { get; }
        public string Flag { get; }

        public SkitEntryData[] SkitEntryData { get; }

        public SkitData(string id, string flag, SkitEntryData[] skitEntryData)
        {
            Id = id;
            Flag = flag;
            SkitEntryData = skitEntryData;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Flag: {Flag}, SkitEntryData: {SkitEntryData}";
        }
    }

    public class SkitEntryData
    {
        public SkitTalkCharaData[] TalkCharaData { get; } //キャラの立ち位置などをまとめたデータ
        public string TalkSpeaker { get; } //話しているキャラの名前
        public string TalkBackground { get; } //背景画像の名前
        
        public string JapaneseTalkDialogue { get; protected set; } //日本語の会話
        
        public string EnglishTalkDialogue { get; } //英語の会話

        public SkitEntryData()
        {
            
        }
        
        public SkitEntryData(SkitTalkCharaData[] talkCharaData, string talkSpeaker, string talkBackground,
            string japaneseTalkDialogue, string englishTalkDialogue)
        {
            TalkCharaData = talkCharaData;
            TalkSpeaker = talkSpeaker;
            TalkBackground = talkBackground;
            JapaneseTalkDialogue = japaneseTalkDialogue;
            EnglishTalkDialogue = englishTalkDialogue;
        }
     
        public override string ToString()
        {
            return $"TalkCharaData: {TalkCharaData}, TalkSpeaker: {TalkSpeaker}, TalkBackground: {TalkBackground}, JapaneseTalkDialogue: {JapaneseTalkDialogue}, EnglishTalkDialogue: {EnglishTalkDialogue}";
        }
    }

    public enum StandingPosition
    {
        None,
        Left,
        Middle,
        Right,
    }

    public class SkitTalkCharaData
    {
        public string CharaName { get; }
        public StandingPosition StandingPosition { get; }
        public string CharaStateFileName { get; }
        
        public SkitTalkCharaData(string charaName, StandingPosition standingPosition, string charaStateFileName)
        {
            CharaName = charaName;
            StandingPosition = standingPosition;
            CharaStateFileName = charaStateFileName;
        } 
    }

    #endregion

    #region 選択肢データ

    /// <summary>
    /// 会話内の各選択肢のデータ
    /// </summary>
    [Serializable]
    public class SkitChoiceData : SkitEntryData
    {
        public string Id { get; }

        public string Answer { get; }
        public float AddPoint { get; }

        public float ChoiceTime { get; }

        public ChoiceEntry[] ChoiceEntries { get; }

        public SkitChoiceData(
            string choiceId,
            float addPoint,
            float choiceTime,
            string answer,
            ChoiceEntry[] choiceEntries,
            string problemDialogue,
            SkitTalkCharaData[] talkCharaData,
            string talkSpeaker,
            string talkBackground,
            string englishTalkDialogue
        ) : base(talkCharaData, talkSpeaker, talkBackground, problemDialogue, englishTalkDialogue)
        {
            Id = choiceId;
            AddPoint = addPoint;
            ChoiceTime = choiceTime;
            Answer = answer;
            ChoiceEntries = choiceEntries;
        }

        public SkitChoiceData(
            string choiceId,
            float addPoint,
            float choiceTime,
            string answer, ChoiceEntry[] choiceEntries, string problemDialogue)
        {
            Id = choiceId;
            AddPoint = addPoint;
            ChoiceTime = choiceTime;
            Answer = answer;
            ChoiceEntries = choiceEntries;
            JapaneseTalkDialogue = problemDialogue;
        }
    }

    [Serializable]
    public class ChoiceEntry
    {
        public string ChoiceEntryId { get; }
        public  string EnglishChoiceEntryName { get; }
        public string JapaneseChoiceEntryName { get; }

        public ChoiceEntry(string choiceEntryId, string englishChoiceEntryName, string japaneseChoiceEntryName)
        {
            ChoiceEntryId = choiceEntryId;
            EnglishChoiceEntryName = englishChoiceEntryName;
            JapaneseChoiceEntryName = japaneseChoiceEntryName;
        }
    }

    #endregion
    
    #region チュートリアルデータ
    [Serializable]
    public class TutorialData : ISkitSceneData
    {
        public string Id { get; }
        public string Flag { get; }
        public string[] JapaneseDialogue { get; }
        public string BackgroundImageName { get; }
        
        public TutorialData(string tutorialId, string flag, string[] japaneseDialogue, string backgroundImageName)
        {
            Id = tutorialId;
            Flag = flag;
            JapaneseDialogue = japaneseDialogue;
            BackgroundImageName = backgroundImageName;
        }
    }

    public class TutorialChoiceData : SkitChoiceData
    {
        public TutorialChoiceData(string choiceId, float addPoint, float choiceTime, string answer, ChoiceEntry[] choiceEntries,
            string problemDialogue, SkitTalkCharaData[] talkCharaData, string talkSpeaker, string talkBackground,
            string englishTalkDialogue, string tutorialDialog) : base(choiceId, addPoint, choiceTime,  answer, choiceEntries,
            problemDialogue, talkCharaData,
            talkSpeaker, talkBackground, englishTalkDialogue)
        {
        }
    }

    public class TutorialClassSelectData : ClassSelectData
    {
        public TutorialClassSelectData(string classSelectId, string flag, int s, string classSelectTalkerName,
            string classSelectBackgroundImageName, string classSelectDialogue, ClassChoiceData[] classChoices,
            string tutorialDialog) : base(classSelectId, flag, s, classSelectTalkerName, classSelectBackgroundImageName,
            classSelectDialogue, classChoices){}
    }

    public class NormalTutorialData
    {
        public string JapaneseDialogue { get; }
        public string BackgroundImageName { get; }
        
        public NormalTutorialData(string japaneseDialogue, string backgroundImageName)
        {
            JapaneseDialogue = japaneseDialogue;
            BackgroundImageName = backgroundImageName;
        }
    }

    #endregion
}