using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using TeamB.TalkSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.TalkDebug
{
    public class TalkDebug : MonoBehaviour
    {
        [SerializeField] private RectTransform _classSelectPanel = null;
        [SerializeField] private Button _debugButtonPrefab = null;
        [SerializeField] private TalkSystemManager _talkSystemManager = null;
        [SerializeField] private RectTransform _debugPanel = null;
        [SerializeField] private TalkSystemManager.LoadType _loadType = TalkSystemManager.LoadType.Local;
        private ITalkDataLoader _talkDataLoader = null;

        private async void Awake()
        {
            _talkSystemManager.enabled = false;
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => ActiveDebugPanel(!_debugPanel.gameObject.activeSelf)).AddTo(this);
            await SetTalkDataLoader();
            SetClassSelectDebugButton();
        }
        
        private void ActiveDebugPanel(bool isActive)
        {
            this.gameObject.SetActive(isActive);
        }

        private async UniTask SetTalkDataLoader()
        {
            if (_loadType == TalkSystemManager.LoadType.Remote)
            {
                _talkDataLoader = new RemoteTalkDataLoader();
            }
            else
            {
                _talkDataLoader = new LocalTalkDataLoader();
            }

            await _talkDataLoader.InitTalkData();
            _talkSystemManager.SetTalkDataLoader(_talkDataLoader);
        }

        private void SetClassSelectDebugButton()
        {
            if (_talkDataLoader.TryGetAllClassSelectData(out var classSelectDataList)) return;
            foreach (var data in classSelectDataList)
            {
                var button = Instantiate(_debugButtonPrefab, _classSelectPanel);
                button.GetComponentInChildren<TextMeshProUGUI>().text = data.ClassSelectId;
                button.onClick.AddListener(() =>
                {
                    _talkSystemManager.SetClassSelectData(data);
                    ActiveDebugPanel(false);
                    _talkSystemManager.enabled = true;
                });
            }
        }

        private void Reset()
        {
            _talkSystemManager = FindObjectOfType<TalkSystemManager>();
        }
    }
    
}
