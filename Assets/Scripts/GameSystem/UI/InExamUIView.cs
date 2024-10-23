using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;

namespace TeamB.UI
{
    /// <summary>
    /// 試験のUIを管理する
    /// </summary>
    public class InExamUIView : UIView
    {
        /// <summary>
        /// 試験を終えるタイミングで呼び出す
        /// </summary>
        public void ExitExam()
        {
            // 乱数をはじく
            var tmp = Random.Range(1, 10);
            if (tmp > 5)
            {
                SceneLoader.LoadScene("moch_CharmUp");
                GameStatics.PrevGameState = GameState.Exam;
            }
            else
            {
                SceneLoader.LoadScene("moch_SuddenlyEvent");
                GameStatics.PrevGameState = GameState.Exam;
            }
        }
    }
}