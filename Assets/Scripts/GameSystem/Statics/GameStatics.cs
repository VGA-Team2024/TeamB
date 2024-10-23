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

        public static void Log()
        {
            Debug.Log("試験まで残り:" + RemainingDayForExam + "日");
        }
    }
}