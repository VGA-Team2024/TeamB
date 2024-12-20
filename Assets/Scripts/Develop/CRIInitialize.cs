using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.Develop
{
    public class CRIInitialize : MonoBehaviour
    {
        private void Awake()
        {
            CRIAudioManager.Initialize();
        }
    }
}
