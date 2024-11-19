using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using TGS2023.BGM;
using UISystem;
using UnityEngine;

namespace TeamB.UI
{
    public class TitleUIView : UIView
    {
        protected override void AwakeCall()
        {
            //Awake時にやる処理を書く
            CRIAudioManager.Initialize();
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_title));
        }

        /// <summary>
        /// ゲームを開始する
        /// </summary>
        public void GameStart()
        { 
            SceneLoader.LoadScene("Skit");
            GameStatics.PrevGameState = GameState.Title;
        }

        public void SceneChange(string sceneName)
        {
            SceneLoader.LoadScene(sceneName);
        }
    }
}