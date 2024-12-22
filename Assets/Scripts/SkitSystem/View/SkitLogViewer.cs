using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitLogViewer : MonoBehaviour
    {
        [SerializeField] private RectTransform _skitLogPanelParent;
        [SerializeField] private GameObject _skitLogTextPrefab;
        [SerializeField] private ScrollRect _logScrollRect;
        [SerializeField] private SkitBackgroundTapSensor _skitBackgroundTapSensor;
        [SerializeField] private RectTransform[] _logObjects;
        public bool IsLogActive => _logObjects[0].gameObject.activeSelf;

        private void Start()
        {
            SetActivePanel(false);
            
        }

        public void SetLog(SkitEntryData skitEntryData)
        {
            var logItem = Instantiate(_skitLogTextPrefab, _skitLogPanelParent);
            var logText = logItem.GetComponentsInChildren<TMP_Text>();
            if (skitEntryData is SkitChoiceData skitChoiceData)
            {
                logText[0].text = skitChoiceData.TalkSpeaker;
                logText[1].text = skitEntryData.JapaneseTalkDialogue;
            }
            else
            {
                logText[0].text = skitEntryData.TalkSpeaker;
                logText[1].text = skitEntryData.JapaneseTalkDialogue;
            }
        }

        public void SetUserAnswerLog(string answer)
        {
            var logItem = Instantiate(_skitLogTextPrefab, _skitLogPanelParent);
            var logText = logItem.GetComponentsInChildren<TMP_Text>();
            logText[1].text = answer;
            logText[1].color = Color.red;
        }
        
        public void SetChoiceLog(SkitEntryData skitEntryData, string choiceText)
        {
            var logItem = Instantiate(_skitLogTextPrefab, _skitLogPanelParent);
            var logText = logItem.GetComponentsInChildren<TMP_Text>();
            logText[0].text = skitEntryData.TalkSpeaker;
            logText[1].text = choiceText;
        }
        
        public void SetActivePanel(bool isActive)
        {
            foreach (var logObject in _logObjects)
            {
                logObject.gameObject.SetActive(isActive);
            }
            _logScrollRect.verticalNormalizedPosition = 0f;
            if (isActive)
            {
                _skitBackgroundTapSensor.OnBackgroundTap += () => SetActivePanel(false);
            }
        }
    }
}
