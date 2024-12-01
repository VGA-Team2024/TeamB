using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.SkitSystem
{
    [CreateAssetMenu(fileName = "SkitFlagData", menuName = "SkitSystem/SkitFlagData")]
    public class SkitFlagData : ScriptableObject
    {
        [SerializeField] private string _currentFlag;
        public string CurrentFlag => _currentFlag;
        
        public void SetCurrentFlag(string flag)
        {
            _currentFlag = flag;
        }
    }
}
