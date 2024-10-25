using System;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.UI
{
    /// <summary>
    /// 授業中のUIを管理する
    /// </summary>
    public class InClassUIView : UIView
    {
        [SerializeField] private Text _remainingDays;
        private const string _nokori = "のこり";

        private void Start()
        {
            _remainingDays.text = _nokori + GameStatics.RemainingDayForExam + "日";
        }

        /// <summary>
        /// 授業をおえる
        /// </summary>
        public void EndClass()
        {
            if (GameStatics.RemainingDayForExam > 1)
            {
                GameStatics.RemainingDayForExam--;

                _remainingDays.text = _nokori + GameStatics.RemainingDayForExam + "日";
            }
            else
            {
                GameStatics.RemainingDayForExam = 3;

                _remainingDays.text = _nokori + GameStatics.RemainingDayForExam + "日";
                SceneLoader.LoadScene("moch_Exam");
            }

            GameStatics.PrevGameState = GameState.Class;
        }
    }
}