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
            if (GameStatics.ExamState == ExamState.FirstExam)
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
            if (GameStatics.ExamState == ExamState.FirstExam)
            {
                SceneLoader.LoadScene("moch_Talk");
            }
            else
            {
                SceneLoader.LoadScene("Title");
            }
        }
    }
}