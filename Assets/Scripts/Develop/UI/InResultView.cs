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

        private void Start()
        {
            if (GameStatics.ExamState == ExamState.FirstExam)
            {
                _text.text = "不合格";
            }
            else
            {
                _text.text = "合格";
            }

            _text.GetComponent<RectTransform>().transform.DOScale(new Vector3(300, 300, 300), 0f);
            _text.GetComponent<RectTransform>().transform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutCirc);
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