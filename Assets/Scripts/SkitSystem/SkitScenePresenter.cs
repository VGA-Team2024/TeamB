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
        [SerializeField] private TestSkitFlagData _testSkitFlagData;
        private async void Awake()
        {
            _loadingPanel.SetActive(true);
            await _skitResourceLoader.InitializeSkitResourceLoader();
            var classSelectSkitContextHandler = new ClassSelectSkitContextHandler();
            var skitDataHandler = new SkitDataHandler();
            var skitChoiceHandler = new SkitChoiceHandler();
            _skitSystemManager.SetSkitContextHandlers(classSelectSkitContextHandler);
            _skitSystemManager.SetSkitContextHandlers(skitDataHandler);
            _skitSystemManager.SetSkitContextHandlers(skitChoiceHandler);
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

            if (!_testSkitFlagData.Prologue)
            {
                _skitSystemManager.SetTestSkitId("01_prologue1");
                _testSkitFlagData.Prologue = true;
            }
            else if (_testSkitFlagData.Prologue && _testSkitFlagData.FirstExamClear)
            {
                _skitSystemManager.SetTestSkitId("01_FirstExam2");
            }
            else if (_testSkitFlagData.Prologue && !_testSkitFlagData.FirstExamClear)
            {
                _skitSystemManager.SetTestSkitId("01_FirstExam3");
            }
            else if (_testSkitFlagData.Prologue && _testSkitFlagData.FirstExamClear && _testSkitFlagData.SecondExamClear)
            {
                _skitSystemManager.SetTestSkitId("01_SecondExam2");
            }
            else
            {
                _skitSystemManager.SetTestSkitId("01_SecondExam3");
            }
            
            await _skitSystemManager.Initialize();
            _loadingPanel.SetActive(false);
            _skitSystemManager.DoSkitSequence().Forget();
        }

        private void Start()
        {
            CRIAudioManager.Initialize();
            CRIAudioManager.BGM.Stop();
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));
        }
    }
}
