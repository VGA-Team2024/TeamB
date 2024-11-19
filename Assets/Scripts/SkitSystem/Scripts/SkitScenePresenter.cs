using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using TGS2023.BGM;

namespace TeamB.SkitSystem
{
    public class SkitScenePresenter : MonoBehaviour
    {
        [SerializeField] private SkitSystemManager _skitSystemManager;
        [SerializeField] private SkitView _skitSceneView;
        [SerializeField] private SkitResourceLoader _skitResourceLoader;
        [SerializeField] private GameObject _loadingPanel;
        // Start is called before the first frame update
        private async void Awake()
        {
            _loadingPanel.SetActive(true);
            var classSelectSkitContextHandler = new ClassSelectSkitContextHandler();
            var skitDataHandler = new SkitDataHandler();
            var skitChoiceHandler = new SkitChoiceHandler();
            _skitSystemManager.SetSkitContextHandlers(classSelectSkitContextHandler);
            _skitSystemManager.SetSkitContextHandlers(skitDataHandler);
            _skitSystemManager.SetSkitContextHandlers(skitChoiceHandler);
            await _skitResourceLoader.InitializeSkitResourceLoader();
            _skitSceneView.SetSkitResourceLoader(_skitResourceLoader);

            skitDataHandler.CurrentSkitEntryData.Subscribe(skitEntryData =>
            {
                if (skitEntryData == null) return;
                _skitSceneView.SetCharacterAndBackground(skitEntryData.TalkBackground, skitEntryData.TalkCharaData);
                _skitSceneView.ShowDialogue(skitEntryData.TalkSpeaker, skitEntryData.JapaneseTalkDialogue).Forget();
            }).AddTo(_skitSceneView);

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ =>
                {
                    skitDataHandler.AwaitForNextUts?.TrySetResult(("", ""));
                }).AddTo(this);

            _loadingPanel.SetActive(false);
        }

        private void Start()
        {
            CRIAudioManager.Initialize();
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));
        }
    }
}
