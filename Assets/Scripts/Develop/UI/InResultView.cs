using DG.Tweening;
using TeamB.Data;
using TeamB.GameSystem.Statics;
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
        [SerializeField] TMP_Text _text;
        private string _passedSentence = "合格";
        private string _notPassedSentence = "不合格";
        Vector3 _startScale = new Vector3(300, 300, 300);
        private float _stampTime = 1.5f;

        protected override void AwakeCall()
        {
            if (GameStatics.ExamResult == ExamResult.Clear)
            {
                _text.text = _passedSentence;
            }
            else
            {
                _text.text = _notPassedSentence;
            }

            CRIAudioManager.Initialize();
            if (GameStatics.ExamResult == ExamResult.Failed)
                CRIAudioManager.SE.Play("SE", nameof(SE.SE_GO));
            else
                CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));
            _text.GetComponent<RectTransform>().transform.DOScale(_startScale, 0f);
            _text.GetComponent<RectTransform>().transform.DOScale(Vector3.one, _stampTime).SetEase(Ease.OutCirc);
        }

        public void Result()
        {
            string sceneName = "";
            switch (GameStatics.ExamState)
            {
                case ExamState.FirstExam:
                    sceneName = "Talk";
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
            SceneLoader.LoadScene(sceneName);
        }

        public void ClickSound()
        {
            CRIAudioManager.SE.Play("SE", nameof(SE.SE_click));
        }
    }
}