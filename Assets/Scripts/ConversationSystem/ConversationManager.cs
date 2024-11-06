using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using UnityEngine;

namespace TeamB.ConversationSystem
{
    public interface IConversationManager
    {
        void InitData();
        bool TryGetConversationEntryData(out ConversationEntry conversationEntry);
    }
    
    [Serializable]
    public class ConversationManager : IConversationManager
    {
        [SerializeField] private ConversationData _conversationData = null;
        private int _currentConversationIndex = 0;

        public void InitData()
        {
            //todo: 会話データをロードする処理を追加
            if (_conversationData == null)
            {
                Debug.LogError("ConversationData is null");
                return;
            }
        }

        /// <summary>
        /// 会話データを取得する。取得時にインデックスを進める
        /// </summary>
        /// <param name="conversationEntry"></param>
        /// <returns></returns>
        public bool TryGetConversationEntryData(out ConversationEntry conversationEntry)
        {
            if (_conversationData == null)
            {
                Debug.LogError("ConversationData is null");
                conversationEntry = null;
                return false;
            }
            
            if (_currentConversationIndex >= _conversationData.ConversationEntries.Count)
            {
                conversationEntry = null;
                return false;
            }
            conversationEntry = _conversationData.ConversationEntries[_currentConversationIndex];
            _currentConversationIndex++;
            return true;
        }
    }
}
