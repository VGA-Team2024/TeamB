using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// 各イベントごとの会話データをまとめたクラス
    /// </summary>
    [CreateAssetMenu(fileName = "ConversationData", menuName = "Conversation/Create Conversation Data", order = 1)]
    public class ConversationData : ScriptableObject
    {
        public string ConversationName;
        public List<ConversationEntry> ConversationEntries = new List<ConversationEntry>();
    }

    /// <summary>
    /// 会話データ
    /// </summary>
    [System.Serializable]
    public class ConversationEntry
    {
        public Speaker Speaker;
        public string Dialogue;
        public CharacterData Character1;
        public CharacterData Character2;
    }

    /// <summary>
    /// 会話時のキャラの状態をまとめたクラス
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        public Speaker CharacterName;
        public Position Position;
        public AnimationState Animation;
    }

    /// <summary>
    /// キャラのアニメーションをEnumで管理するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationStateInfo", menuName = "Conversation/Create AnimationState Data", order = 2)]
    public class AnimationStateInfo : ScriptableObject
    {
        public Speaker Speaker;
        public List<AnimationStateData> AnimationStateDatas = new List<AnimationStateData>();
    }

    [System.Serializable]
    public class AnimationStateData
    {
        public AnimationState AnimationState;
        //ToDo: 再生するアニメーションの方式に合わせて直接指定できるようにする
        public string AnimationName;
    }

    public enum Speaker { Speaker1, Speaker2 }
    public enum Position { Left, Center, Right }
    public enum AnimationState { Idle, Talking, Action }
}

