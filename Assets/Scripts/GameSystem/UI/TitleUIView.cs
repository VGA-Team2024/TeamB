using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;

namespace TeamB.UI
{
    public class TitleUIView : UIView
    {
        protected override void AwakeCall()
        {
            //Awake時にやる処理を書く
        }

        /// <summary>
        /// ゲームを開始する
        /// </summary>
        public void GameStart()
        {
            // 乱数をはじく
            var tmp = Random.Range(1, 10);
            if (tmp > 5)
            {
                SceneLoader.LoadScene("moch_Talk");
            }
            else
            {
                SceneLoader.LoadScene("moch_SuddenlyEvent");
            }

            GameStatics.PrevGameState = GameState.Title;
        }
    }
}