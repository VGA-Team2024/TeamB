using TeamB.Data;
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
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_title));
            
            _skitFlagData.SetCurrentFlag("Prologue");
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

        public void ClickSound()
        {
            CRIAudioManager.SE.Play("SE", nameof(TGS2023.SE.SE.SE_001_enter));
        }
    }
}