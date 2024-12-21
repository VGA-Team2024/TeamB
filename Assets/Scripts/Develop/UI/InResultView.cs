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
        [SerializeField] private Image _passImage;
        [SerializeField] private Image _faildImage;
        private Vector3 _startScale = new Vector3(300, 300, 300);

        private float _stampTime = 1.5f;

        protected override void AwakeCall()
        {
            if (GameStatics.ExamResult == ExamResult.Clear)
            {
                _passImage.gameObject.SetActive(true);
                _passImage.GetComponent<RectTransform>().transform.DOScale(_startScale, 0f);
                _passImage.GetComponent<RectTransform>().transform.DOScale(Vector3.one, _stampTime)
                    .SetEase(Ease.OutCirc);
            }
            else
            {
                _faildImage.gameObject.SetActive(true);
                _faildImage.gameObject.GetComponent<RectTransform>().transform.DOScale(_startScale, 0f);
                _faildImage.gameObject.GetComponent<RectTransform>().transform.DOScale(Vector3.one, _stampTime)
                    .SetEase(Ease.OutCirc);
            }

            CRIAudioManager.BGM.Stop();
            switch (GameStatics.ExamResult)
            {
                case ExamResult.Clear:
                    CRIAudioManager.VOICE.Play("Lian", nameof(Lian.Lian_09));
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
            switch (GameStatics.ExamState)
            {
                case ExamState.ExamClear:
                    GameStatics.ExamState = ExamState.FirstExam;
                    break;
            }

            GameStatics.ExamResult = ExamResult.None;
            SceneLoader.LoadScene("Skit");
        }

        public void ClickSound()
        {
            CRIAudioManager.SE.Play("SE", nameof(TGS2023.SE.SE.SE_001_enter));
        }
    }
}