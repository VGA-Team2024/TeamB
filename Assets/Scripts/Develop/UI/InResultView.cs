using System.Collections;
using System.Collections.Generic;
using CodiceApp.Gravatar;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;

namespace TeamB.Develop
{
    public class InResultView : UIView
    {
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
