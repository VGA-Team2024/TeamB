using DG.Tweening;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using TeamB.SkitSystem;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class InResultView : UIView
    {
        [SerializeField] UnityEngine.UI.Text _text;
        [SerializeField] TestSkitFlagData _testSkitFlagData;
        private string _passedSentence = "合格";
        private string _notPassedSentence = "不合格";
        Vector3 _startScale = new Vector3(300, 300, 300);
        private float _stampTime = 1.5f;

        private void Start()
        {
            if (GameStatics.ExamResult == ExamResult.Clear)
            {
                _text.text = _passedSentence;
            }
            else
            {
                _text.text = _notPassedSentence;
            }

            _text.GetComponent<RectTransform>().transform.DOScale(_startScale, 0f);
            _text.GetComponent<RectTransform>().transform.DOScale(Vector3.one, _stampTime).SetEase(Ease.OutCirc);
            
            SetTestFlag();
        }

        /// <summary>
        /// テスト用のフラグを立てるためのメソッドです。
        /// </summary>
        private void SetTestFlag()
        {
            if (_testSkitFlagData.Prologue && GameStatics.ExamResult == ExamResult.Clear)
            {
                _testSkitFlagData.FirstExamClear = true;
            }
            else if (_testSkitFlagData.Prologue && _testSkitFlagData.FirstExamClear && GameStatics.ExamResult == ExamResult.Clear)
            {
                _testSkitFlagData.SecondExamClear = true;
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
                        sceneName = "GameOver";
                    break;
                case ExamState.ExamClear:
                    sceneName = "Title";
                    GameStatics.ExamState = ExamState.FirstExam;
                    break;
            }

            GameStatics.ExamResult = ExamResult.None;
            SceneLoader.LoadScene("Skit");
        }
    }
}