using DG.Tweening;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class InResultView : UIView
    {
        [SerializeField] UnityEngine.UI.Text _text;
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
        }

        public void Result()
        {
            string sceneName = "";
            switch (GameStatics.ExamState)
            {
                case ExamState.FirstExam:
                    sceneName = "TalkTest";
                    break;
                case ExamState.SecondExam:
                    if (GameStatics.ExamResult == ExamResult.Clear)
                        sceneName = "Exam";
                    else if (GameStatics.ExamResult == ExamResult.Failed)
                        sceneName = "GameOver";
                    break;
                case ExamState.ExamClear:
                    sceneName = "Title";
                    break;
            }

            GameStatics.ExamResult = ExamResult.None;
            SceneLoader.LoadScene(sceneName);
        }
    }
}