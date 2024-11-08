using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.ConversationSystem;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TeamB.Data;
using TeamB.GameSystem.Statics;

namespace TeamB.ConversationSystem
{
    public class ConversationPresenter : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private IConversationManager _conversationManager = null;
        [SerializeReference, SubclassSelector] private IConversationView _conversationView = null;
        [SerializeField] private ClassSelectManager _classSelectManager = null;
        [SerializeField] private ClassSelectView _classSelectView = null;

        private async void Start()
        {
            //クラス選択画面
            _conversationView.SetSpeakerText("先生");
            _conversationView.SetDialogueText("今日の授業を選んでください");
            _conversationView.OnCloseConversationView();
            _classSelectView.OnOpenClassSelectView();
            var taskCompletionSource = new UniTaskCompletionSource<(string id, string reward)>();
            _classSelectView.SetChoiceButtons(_classSelectManager.GetChoiceTexts(), _classSelectManager.GetConversationIDs(), _classSelectManager.GetRewardTexts(), taskCompletionSource);
            var choice = await taskCompletionSource.Task;
            _classSelectView.OnCloseClassSelectView();
            
            //授業の会話シーン
            _conversationView.OnOpenConversationView();
            _conversationManager.SetConversationDataById(choice.id);
            _conversationManager.InitData();
            _conversationView.SetBackground(_conversationManager.GetConversationData()?.Background);
            while (_conversationManager.TryGetConversationEntryData(out var conversationEntry))
            {
                _conversationView.ResetCharaImages();
                _conversationView.SetSpeakerText(conversationEntry.Speaker);
                conversationEntry.Characters.ForEach(x => _conversationView.SetCharacterData(x));
                await _conversationView.SetDialogueText(conversationEntry.Dialogue);
                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            }
            SceneLoader.LoadScene("Exam");
            GameStatics.PrevGameState = GameState.Class;
        }
    }
}
