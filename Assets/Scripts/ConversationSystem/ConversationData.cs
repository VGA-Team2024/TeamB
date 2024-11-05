using System.Collections.Generic;
using UnityEngine;

namespace TeamB.Data
{
    /// <summary>
    /// 各イベントごとの会話データをまとめたクラス
    /// </summary>
    [CreateAssetMenu(fileName = "ConversationData", menuName = "Conversation/Create Conversation Data", order = 1)]
    public class ConversationData : ScriptableObject
    {
        [Editor.ReadOnly, InspectorVariantName("イベントID")] public string ConversationName;
        public List<ConversationEntry> ConversationEntries = new List<ConversationEntry>();
    }

    /// <summary>
    /// 会話データ
    /// </summary>
    [System.Serializable]
    public class ConversationEntry
    {
        [Editor.ReadOnly, InspectorVariantName("話者")] public string Speaker;
        [Editor.ReadOnly, InspectorVariantName("会話")] public string Dialogue;
        public List<CharacterData> Characters;
    }

    /// <summary>
    /// 会話時のキャラの状態をまとめたクラス
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        [Editor.ReadOnly, InspectorVariantName("キャラ名")] public string CharacterName;
        [Editor.ReadOnly, InspectorVariantName("位置")] public Position Position;
        [Editor.ReadOnly, InspectorVariantName("アニメーション名")] public string Animation;
    }

    /// <summary>
    /// キャラのアニメーションをEnumで管理するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationStateInfo", menuName = "Conversation/Create AnimationState Data", order = 2)]
    public class AnimationStateInfo : ScriptableObject
    {
        public string Speaker;
        public List<AnimationStateData> AnimationStateData = new List<AnimationStateData>();
    }

    [System.Serializable]
    public class AnimationStateData
    {
        public string AnimationState;

        //ToDo: 再生するアニメーションの方式に合わせて直接指定できるようにする
        public string AnimationName;
    }

    public enum Position
    {
        Left,
        Middle,
        Right
    }
}