using System;
using System.Linq;
using DG.Tweening;
using SE.Lian;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using TeamB.SkitSystem;
using TGS2023.BGM;
using TGS2023.SE;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class InResultView : UIView
    {
        [SerializeField] private ExamStateDatas examState;
        [SerializeField] private SkitFlagData _skitFlagData;
        private Vector3 _startScale = new Vector3(300, 300, 300);

        private float _stampTime = 1.5f;

        protected override void AwakeCall()
        {
            CRIAudioManager.BGM.Stop();
            switch (GameStatics.ExamResult)
            {
                case ExamResult.Clear:
                    CRIAudioManager.VOICE.Play("Lian", nameof(SE.Lian.Lian.Lian_09));
                    break;
                default:
                    CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));
                    break;
            }


            SetTestFlag();
        }

        /// <summary>
        /// テスト用のフラグを立てるためのメソッドです。
        /// </summary>
        private void SetTestFlag()
        {
        }

        public void Result()
        {
            GameStatics.ExamResult = ExamResult.None;
            SceneLoader.LoadScene("Skit");
        }

        public void ClickSound()
        {
            CRIAudioManager.SE.Play("SE", nameof(TGS2023.SE.SE.SE_001_enter));
        }
    }
}