using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.SkitSystem
{
    [CreateAssetMenu(fileName = "TestSkitFlagData", menuName = "SkitSystem/TestSkitFlagData")]
    public class TestSkitFlagData : ScriptableObject
    {
        public bool Prologue;
        public bool FirstExamClear;
        public bool SecondExamClear;
    }
}
