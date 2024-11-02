using System.Collections.Generic;
using DataManagement.SpreadSheet;
using TeamB.Data;
using UnityEngine;
using CharacterMaster = DataManagement.CharacterMaster;

namespace TeamB.GameSystem.Statics
{
    /// <summary>
    /// ゲーム内で扱う静的なフィールドをここに集約する
    /// </summary>
    public static class GameStatics
    {
        /// <summary>
        /// 試験までの残り日数
        /// </summary>
        public static int RemainingDayForExam = 3;

        public static GameState PrevGameState;

        /// <summary>
        /// 育成キャラの現ステータス
        /// </summary>
        public static Dictionary<CharacterType, DataManagement.SpreadSheet.CharacterData> NurturingCharacter = new();
        
        public static void Log()
        {
            Debug.Log("試験まで残り:" + RemainingDayForExam + "日");
        }
    }
}