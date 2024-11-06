using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.ConversationSystem
{
    [CreateAssetMenu(fileName = "ConversationCharaData", menuName = "Conversation/Create Conversation Chara Data", order = 3)]
    public class ConversationCharaData : ScriptableObject
    {
        public string CharacterName;
        public Animator Animator;
    }
}
