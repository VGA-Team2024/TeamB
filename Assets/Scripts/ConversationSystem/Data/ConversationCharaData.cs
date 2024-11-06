using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.ConversationSystem
{
    [CreateAssetMenu(fileName = "ConversationCharaData", menuName = "Conversation/Create Conversation CharaData", order = 1)]
    public class ConversationCharaData : ScriptableObject
    {
        public string CharaName;
        public string JapaneseCharaName;
        public List<AnimationDictionary> AnimationDictionary = new ();
    }

    [Serializable]
    public class AnimationDictionary
    {
        public string AnimationKey;
        public Sprite AnimationValue;
    }
}
