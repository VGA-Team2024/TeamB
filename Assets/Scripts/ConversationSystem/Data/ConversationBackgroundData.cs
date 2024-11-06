using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.ConversationSystem
{
    [CreateAssetMenu(fileName = "ConversationBackgroundData", menuName = "ConversationBackgroundData", order = 0)]
    public class ConversationBackgroundData : ScriptableObject
    {
        public string BackgroundID;
        public Sprite BackgroundImage;
    }
}
