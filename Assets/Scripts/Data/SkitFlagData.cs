using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.SkitSystem
{
    [CreateAssetMenu(fileName = "SkitFlagData", menuName = "SkitSystem/SkitFlagData")]
    public class SkitFlagData : ScriptableObject
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
