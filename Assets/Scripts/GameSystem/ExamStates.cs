using UnityEngine;
using System;
using System.Collections.Generic;

namespace TeamB.GameSystem
{
    [CreateAssetMenu(fileName = @"ExamStateData",
        menuName = "ScriptableObject/ExamStateData")]
    public class ExamStateDatas : ScriptableObject
    {
        public List<ExamStateData> Data = new();
    }

    [Serializable]
    public class ExamStateData
    {
        public string CurrentState;
        public string ClearState;
        public string FailureState;
    }
}