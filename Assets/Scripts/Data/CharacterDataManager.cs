using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using DataManagement;
using TeamB.GameSystem.Statics;
using UnityEditor;
using UnityEngine;

namespace TeamB.Data
{
    /// <summary>
    /// 育成キャラのデータを管理するクラス
    /// </summary>
    public class CharacterDataManager : MonoBehaviour
    {
        public event Action OnParamUpdated;

        private const string filePath = @"Assets\Scripts\Data\CharacterType.cs";

        /// <summary> パラメータ更新 </summary>
        public DataManagement.SpreadSheet.CharacterData UpdateParam(CharacterType characterType,
            CharacterStatusType paramType, float value)
        {
            if (GameStatics.NurturingCharacter[characterType] == null)
                return null;
            switch (paramType)
            {
                case CharacterStatusType.Rank:
                    GameStatics.NurturingCharacter[characterType].Rank += (int)value;
                    break;
                case CharacterStatusType.Hp:
                    GameStatics.NurturingCharacter[characterType].Hp += value;
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
        /// マスターデータの読み込み
        /// </summary>
        public async UniTask SetUp()
        {
            FileManager fileManager = new FileManager();
            DataManagement.SpreadSheet.CharacterMaster masterData =
                await fileManager.ReadDataAsync<DataManagement.SpreadSheet.CharacterMaster>(GameConsts.CharacterFile,
                    async obj =>
                    {
                        for (int i = 0; i < obj.Data.Length; i++)
                        {
                            GameStatics.NurturingCharacter.Add((CharacterType)obj.Data[i].Id, obj.Data[i]);
                        }
                    });
        }


        /// <summary>
        /// キャラクターの中身をstringに書き出す
        /// </summary>
        /// <param name="playerStatus"></param>
        /// <returns></returns>
        public string PrintCharacterData(CharacterType characterType)
        {
            string playerstatus = $"CharacterData\n" +
                                  $"Name:{GameStatics.NurturingCharacter[characterType].Name},\n" +
                                  $"HP:{GameStatics.NurturingCharacter[characterType].Hp}" +
                                  $"Rank:{GameStatics.NurturingCharacter[characterType].Rank},\n" +
                                  $"HitRate:{GameStatics.NurturingCharacter[characterType].HitRate},\n" +
                                  $"ChantingSpeed:{GameStatics.NurturingCharacter[characterType].ChantingSpeed},\n" +
                                  $"MagicalAmount:{GameStatics.NurturingCharacter[characterType].MagicATK},\n";
            return playerstatus;
        }
    }


    public enum CharacterStatusType
    {
        Rank,
        Hp,
        HitRate,
        ChantingSpeed,
        MagicATK,
        None,
    }
}