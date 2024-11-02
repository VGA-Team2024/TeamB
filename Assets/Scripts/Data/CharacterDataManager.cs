using System;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 育成キャラのデータを管理するクラス
/// </summary>
public class CharacterDataManager : MonoBehaviour
{
    public event Action OnParamUpdated;
    
    /// <summary> パラメータ更新 </summary>
    public CharacterStatus UpdateParam(CharacterType characterType, CharacterStatusType paramType, float value)
    {
        if (GameStatics.NurturingCharacter[characterType] == null)
            return null;
        switch (paramType)
        {
            case CharacterStatusType.Rank:
                GameStatics.NurturingCharacter[characterType].Rank += (int)value;
                break;
            case CharacterStatusType.ChantingSpeed:
                GameStatics.NurturingCharacter[characterType].ChantingSpeed += value;
                break;
            case CharacterStatusType.HitRate:
                GameStatics.NurturingCharacter[characterType].HitRate += value;
                break;
            case CharacterStatusType.MagicATK:
                GameStatics.NurturingCharacter[characterType].MagicATK += value;
                break;
        }
        OnParamUpdated?.Invoke();
        return GameStatics.NurturingCharacter[characterType];
    }

    
    
    /// <summary>
    /// 育成キャラクターの中身をstringに書き出す
    /// </summary>
    /// <param name="playerStatus"></param>
    /// <returns></returns>
    public string PrintCharacterData(CharacterType characterType)
    {
        string playerstatus = $"CharacterData\n" +
                              $"Name:{GameStatics.NurturingCharacter[characterType].Name},\n" +
                              $"Rank:{GameStatics.NurturingCharacter[characterType].Rank},\n" +
                              $"HitRate:{GameStatics.NurturingCharacter[characterType].HitRate},\n" +
                              $"ChantingSpeed:{GameStatics.NurturingCharacter[characterType].ChantingSpeed},\n" +
                              $"MagicalAmount:{GameStatics.NurturingCharacter[characterType].MagicATK},\n";
        return playerstatus;
    }
}

public enum CharacterType{
    Character1,
    Character2,
    Character3,
    Character4
}

public enum CharacterStatusType
{
    None,
    Rank,
    HitRate,
    ChantingSpeed,
    MagicATK,
}

[System.Serializable]
public class CharacterStatus
{
    public string Name;

    /// <summary>
    /// 等級
    /// </summary>
    public int Rank;

    /// <summary>
    /// 命中力
    /// </summary>
    public float HitRate;

    /// <summary>
    /// 魔法の詠唱速度
    /// </summary>
    public float ChantingSpeed;

    /// <summary>
    /// 魔力量
    /// </summary>
    public float MagicATK;


    public CharacterStatus(string name, int rank, float hitRate, float chantingSpeed, float magicAtk)
    {
        Name = name;
        Rank = rank;
        HitRate = hitRate;
        ChantingSpeed = chantingSpeed;
        MagicATK = magicAtk;
    }

    public CharacterStatus()
    {
        Name = "NoName";
        Rank = 1;
        HitRate = 1;
        ChantingSpeed = 1;
        MagicATK = 1;
    }
}