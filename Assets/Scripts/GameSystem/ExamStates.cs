using UnityEngine;
using System;
using System.Collections.Generic;

namespace TeamB.GameSystem
{
    [CreateAssetMenu(fileName = @"ExamStateData",
        menuName = "ScriptableObject/ExamStateData")]
    public class ExamStateDatas : ScriptableObject
    {
        public int Version;
        public List<ExamStateData> Data = new();
    }

    [Serializable]
    public class ExamStateData
    {
        public string ExamDataID;
        public string CurrentState;
        public string VictoryState;
        public string DefeatState;
    }
}