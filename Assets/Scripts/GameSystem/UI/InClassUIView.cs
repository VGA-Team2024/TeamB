using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;

namespace TeamB.UI
{
    /// <summary>
    /// 授業中のUIを管理する
    /// </summary>
    public class InClassUIView : UIView
    {
        /// <summary>
        /// 授業をおえる
        /// </summary>
        public void EndClass()
        {
            if (GameStatics.RemainingDayForExam > 1)
            {
                GameStatics.RemainingDayForExam--;
            }
            else
            {
                GameStatics.RemainingDayForExam = 3;
                SceneLoader.LoadScene("moch_Exam");
            }

            GameStatics.PrevGameState = GameState.Class;
        }
    }
}