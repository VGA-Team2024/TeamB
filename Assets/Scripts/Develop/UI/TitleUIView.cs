using System;
using DG.Tweening;
using TeamB.Data;
using TeamB.Develop;
using TeamB.GameSystem.Statics;
using TeamB.SkitSystem;
using TGS2023.BGM;
using UISystem;
using UnityEngine;

namespace TeamB.UI
{
    public class TitleUIView : UIView
    {
        [SerializeField] private SkitFlagData _skitFlagData;

        protected override void AwakeCall()
        {
            _skitFlagData.SetCurrentFlag("Prologue");
        }

        private async void Start()
        {
            CRIAudioManager.BGM.Stop();
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_001_title));
        }


        /// <summary>
        /// ゲームを開始する
        /// </summary>
        public void GameStart()
        {
            GameEventRecorder.GameStart();
            GameStatics.PrevGameState = GameState.Title;
            SceneLoader.LoadScene("Skit");
        }

        public void SceneChange(string sceneName)
        {
            SceneLoader.LoadScene(sceneName);
        }

        public void ClickSound()
        {
            CRIAudioManager.SE.Play("SE", nameof(TGS2023.SE.SE.SE_001_enter));
        }

        public void ApplicationQuit()
        {
#if UNITY_EDITOR
#else
            Application.Quit();
#endif
        }
    }
}