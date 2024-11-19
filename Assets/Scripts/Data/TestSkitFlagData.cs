using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.SkitSystem
{
    [CreateAssetMenu(fileName = "TestSkitFlagData", menuName = "SkitSystem/TestSkitFlagData")]
    public class TestSkitFlagData : ScriptableObject
    {
        public enum GameState
        {
            Prologue,
            FirstExam,
            FirstExamPassed,
            FirstExamFailed,
            SecondExam,
            SecondExamPassed,
            SecondExamFailed
        }
        public GameState CurrentGameState;
        
    }
}
