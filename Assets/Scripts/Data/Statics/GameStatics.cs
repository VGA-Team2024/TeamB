using System.Collections.Generic;
using DataManagement;
using TeamB.Data;
using UnityEngine;

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
        /// 全キャラの現ステータス
        /// </summary>
        public static Dictionary<int, DataManagement.SpreadSheet.CharacterData> Characters = new();
        
        /// <summary>
        /// 育成しているキャラの種類
        /// </summary>
        public static CharacterType NurturingCharacterType = CharacterType.character1;
        
        /// <summary>
        /// 現在の試験進捗
        /// </summary>
        public static ExamState ExamState = ExamState.Tutorial;
        
        /// <summary>
        /// 言語設定
        /// </summary>
        public static LanguageType Language = LanguageType.Japanese;

        /// <summary>
        /// 倍速設定
        /// </summary>
        public static float TimeScale = 1.0f;

        /// <summary>
        /// 試験クリアしたかどうか
        /// </summary>
        public static ExamResult ExamResult;

        /// <summary>
        /// 試験結果
        /// </summary>
        public static ResultData resultData = new();
        
        public static void Log()
        {
            Debug.Log("試験まで残り:" + RemainingDayForExam + "日");
        }
        
    }
}