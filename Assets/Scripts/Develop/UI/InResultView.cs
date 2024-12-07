using System;
using System.Linq;
using DG.Tweening;
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
        [SerializeField] ExamStateDatas examState;
        [SerializeField] SkitFlagData _skitFlagData;
        [SerializeField] Image _passImage;
        [SerializeField] Image _faildImage;
        Vector3 _startScale = new Vector3(300, 300, 300);
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
            if (GameStatics.ExamResult == ExamResult.Failed)
                CRIAudioManager.SE.Play("SE", nameof(SE.SE_GO));
            else
                CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));


            SetTestFlag();
        }

        /// <summary>
        /// テスト用のフラグを立てるためのメソッドです。
        /// </summary>
        private void SetTestFlag()
        {
            switch (GameStatics.ExamState)
            {
                case ExamState.FirstExam:
                    if (GameStatics.ExamResult == ExamResult.Clear)
                    {
                        GameStatics.ExamState = (ExamState)Enum.Parse(typeof(ExamState),
                            examState.Data.Where(x => x.CurrentState == nameof(ExamState.FirstExam)).First()
                                .VictoryState);
                    }
                    else
                    {
                        GameStatics.ExamState = (ExamState)Enum.Parse(typeof(ExamState),
                            examState.Data.Where(x => x.CurrentState == nameof(ExamState.FirstExam)).First()
                                .DefeatState);
                    }

                    break;
                case ExamState.SecondExam:
                    if (GameStatics.ExamResult == ExamResult.Clear)
                    {
                        GameStatics.ExamState = (ExamState)Enum.Parse(typeof(ExamState),
                            examState.Data.Where(x => x.CurrentState == nameof(ExamState.SecondExam)).First()
                                .VictoryState);
                    }
                    else
                    {
                        GameStatics.ExamState = (ExamState)Enum.Parse(typeof(ExamState),
                            examState.Data.Where(x => x.CurrentState == nameof(ExamState.SecondExam)).First()
                                .DefeatState);
                    }

                    break;
            }
        }

        public void Result()
        {
            string sceneName = "";
            switch (GameStatics.ExamState)
            {
                case ExamState.FirstExam:
                    sceneName = "Skit";
                    break;
                case ExamState.SecondExam:
                    if (GameStatics.ExamResult == ExamResult.Clear)
                        sceneName = "Exam";
                    else if (GameStatics.ExamResult == ExamResult.Failed)
                        sceneName = "Talk";
                    break;
                case ExamState.ExamClear:
                    sceneName = "Title";
                    GameStatics.ExamState = ExamState.FirstExam;
                    break;
            }

            GameStatics.ExamResult = ExamResult.None;
            SceneLoader.LoadScene("Skit");
        }

        public void ClickSound()
        {
            CRIAudioManager.SE.Play("SE", nameof(SE.SE_click));
        }
    }
}