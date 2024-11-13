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
            SceneLoader.LoadScene("Talk");
            GameStatics.PrevGameState = GameState.Title;
        }

        public void SceneChange(string sceneName)
        {
            SceneLoader.LoadScene(sceneName);
        }
    }
}