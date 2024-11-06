using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.ConversationSystem
{
    [CreateAssetMenu(fileName = "ClassSelectData", menuName = "ClassSelectData", order = 0)]
    public class ClassSelectData : ScriptableObject
    {
        public string ClassSelectID;
        public List<string> ChoiceTexts;
        public List<string> ConversationIDs;
        public List<string> RewardTexts; 
    }
}
