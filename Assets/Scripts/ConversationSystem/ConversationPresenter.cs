using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.ConversationSystem;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TeamB.ConversationSystem
{
    public class ConversationPresenter : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private IConversationManager _conversationManager = null;
        [SerializeReference, SubclassSelector] private IConversationView _conversationView = null;

        private async void Start()
        {
            _conversationManager.InitData();

            while (_conversationManager.TryGetConversationEntryData(out var conversationEntry))
            {
                _conversationView.SetSpeakerText(conversationEntry.Speaker);
                _conversationView.SetDialogueText(conversationEntry.Dialogue);
                conversationEntry.Characters.ForEach(x => _conversationView.SetCharacterData(x));
                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            }
        }
    }
}
