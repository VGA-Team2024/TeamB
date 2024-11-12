using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using DataManagement;
using SerializableCollections;
using TeamB.GameSystem.Statics;
using UnityEditor;
using UnityEngine;

namespace TeamB.Data
{
    /// <summary>
    /// 育成キャラのデータを管理するクラス
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class CharacterDataManager
    {
        public static event Action OnParamUpdated;

        private const string filePath = @"Assets\Scripts\Data\CharacterType.cs";

        /// <summary> パラメータ更新 </summary>
        public static DataManagement.SpreadSheet.CharacterData UpdateParam(CharacterType characterType,
            CharacterStatusType paramType, float value)
        {
            if (GameStatics.Characters[(int)characterType] == null)
                return null;
            switch (paramType)
            {
                case CharacterStatusType.Hp:
                    GameStatics.Characters[(int)characterType].Hp += value;
                    break;
                case CharacterStatusType.ChantingSpeed:
                    GameStatics.Characters[(int)characterType].ChantingSpeed += value;
                    break;
                case CharacterStatusType.HitRate:
                    GameStatics.Characters[(int)characterType].HitRate += value;
                    break;
                case CharacterStatusType.MagicATK:
                    GameStatics.Characters[(int)characterType].MagicATK += value;
                    break;
            }

            OnParamUpdated?.Invoke();
            return GameStatics.Characters[(int)characterType];
        }


        /// <summary>
        /// マスターデータの読み込み
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static async UniTask MasterDataSetUp()
        {
            DataManagement.SpreadSheet.CharacterMaster characterData =
                await new CharacterMaster().LoadFromFile("Character");
            for (int i = 0; i < characterData.Data.Length; i++)
            {
                GameStatics.Characters.Add(characterData.Data[i].Id, characterData.Data[i]);
            }
        }


        /// <summary>
        /// キャラクターの全ステータスをstring変換
        /// </summary>
        /// <param name="playerStatus"></param>
        /// <returns></returns>
        public static string PrintCharacterData(CharacterType characterType)
        {
            string playerstatus = $"CharacterData\n" +
                                  $"Name:{GameStatics.Characters[(int)characterType].Name},\n" +
                                  $"HP:{GameStatics.Characters[(int)characterType].Hp}" +
                                  $"HitRate:{GameStatics.Characters[(int)characterType].HitRate},\n" +
                                  $"ChantingSpeed:{GameStatics.Characters[(int)characterType].ChantingSpeed},\n" +
                                  $"MagicalAmount:{GameStatics.Characters[(int)characterType].MagicATK},\n";
            return playerstatus;
        }

        /// <summary>
        /// 各パラメータ更新の情報
        /// </summary>
        /// <param name="characterType"></param>
        /// <param name="paramType"></param>
        /// <param name="value"></param>
        public static string PrintUpdateStatus(CharacterType characterType,
            CharacterStatusType paramType, float value)
        {
            if (GameStatics.Characters[(int)characterType] != null)
            {
                string text = $"{GameStatics.Characters[(int)characterType].Name}";
            }

            return null;
        }
    }


    public enum CharacterStatusType
    {
        Hp,
        HitRate,
        ChantingSpeed,
        MagicATK,
        None,
    }
}