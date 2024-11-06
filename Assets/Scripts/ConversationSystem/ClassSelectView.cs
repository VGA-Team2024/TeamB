using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.ConversationSystem
{
    public class ClassSelectView : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage = null;
        [SerializeField] private Sprite _backgroundSprite = null;
        [SerializeField] private List<ChoiceButton> _choiceButtonPrefab = null;
        [SerializeField] private Sprite _intuitionParameterIcon = null;
        [SerializeField] private Sprite _readingComprehension = null;
        [SerializeField] private Sprite _concentration = null;
        [SerializeField] private List<GameObject> _classSelectViewObjects = null;
        private void SetBackground()
        {
            _backgroundImage.sprite = _backgroundSprite;
        }
        
        public void OnOpenClassSelectView()
        {
            _classSelectViewObjects.ForEach(x => x.SetActive(true));
            SetBackground();
        }
        
        public void SetChoiceButtons(List<string> choiceTexts, List<string> conversationIDs, List<string> rewards, UniTaskCompletionSource<(string, string)> taskCompletionSource)
        {
            for (int i = 0; i < choiceTexts.Count; i++)
            {
                _choiceButtonPrefab[i].gameObject.SetActive(true);
                        Debug.Log(rewards[i]);
                switch (rewards[i])
                {
                    case "直観力":
                        _choiceButtonPrefab[i].SetIcon(_intuitionParameterIcon);
                        break;
                    case "読解力、学力":
                        _choiceButtonPrefab[i].SetIcon(_readingComprehension);
                        break;
                    case "集中力":
                        _choiceButtonPrefab[i].SetIcon(_concentration);
                        break;
                }
                _choiceButtonPrefab[i].SetText(choiceTexts[i]);
                _choiceButtonPrefab[i].Button.onClick.RemoveAllListeners(); 
                var rewardIndex = i;
                _choiceButtonPrefab[i].Button.onClick.AddListener(() =>
                {
                    taskCompletionSource.TrySetResult((conversationIDs[rewardIndex], rewards[rewardIndex]));
                });
            }
        }
        
        public void OnCloseClassSelectView()
        {
            _classSelectViewObjects.ForEach(x => x.SetActive(false));
        }
    }
    
    
}

