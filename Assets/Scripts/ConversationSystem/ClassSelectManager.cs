using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.ConversationSystem
{
    
    public class ClassSelectManager : MonoBehaviour
    {
        [SerializeField] private ClassSelectData _classSelectData = null;
        
        public List<string> GetChoiceTexts()
        {
            return _classSelectData.ChoiceTexts;
        }
        
        public List<string> GetConversationIDs()
        {
            return _classSelectData.ConversationIDs;
        }
        
        public List<string> GetRewardTexts()
        {
            return _classSelectData.RewardTexts;
        }
    }
}
