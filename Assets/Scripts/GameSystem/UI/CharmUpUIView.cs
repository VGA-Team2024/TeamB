using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;

namespace TeamB.UI
{
    /// <summary>
    /// 試験の後のシーンのUIを管理する
    /// </summary>
    public class CharmUpUIView : UIView
    {
        /// <summary>
        /// 一日を終える
        /// </summary>
        public void FinishTheDay()
        {
            SceneLoader.LoadScene("moch_Title");

            GameStatics.PrevGameState = GameState.CharmUp;
        }
    }
}