using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamB.Develop
{
    public class InGameOverUIView : UIView
    {
        public void Next()
        {
            SceneLoader.LoadScene("Title");
        }
    }
}
