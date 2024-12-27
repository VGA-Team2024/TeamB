using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using UnityEngine;

namespace TeamB.Develop
{
    [DefaultExecutionOrder(-100)]
    public class CRIInitialize : MonoBehaviour
    {
        public event Action OnComplete;
        private void Awake()
        {
            CRIAudioManager.Initialize(OnComplete);
        }
    }
}