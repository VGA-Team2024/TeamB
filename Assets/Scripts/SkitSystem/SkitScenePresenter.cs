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
        [SerializeField] private SkitViewFade _loadingPanel;
        [SerializeField] private TestSkitFlagData _testSkitFlagData;
        private async void Awake()
        {
            _loadingPanel.gameObject.SetActive(true);
            _loadingPanel.FadeInAsync(true).Forget();
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

            if (_testSkitFlagData.CurrentGameState == TestSkitFlagData.GameState.Prologue)
            {
                _skitSystemManager.SetTestSkitId("01_prologue1");
                _testSkitFlagData.CurrentGameState = TestSkitFlagData.GameState.FirstExam;
            }
            else if (_testSkitFlagData.CurrentGameState == TestSkitFlagData.GameState.FirstExamPassed)
            {
                _skitSystemManager.SetTestSkitId("01_FirstExam2");
                _testSkitFlagData.CurrentGameState = TestSkitFlagData.GameState.SecondExam;
            }
            else if (_testSkitFlagData.CurrentGameState == TestSkitFlagData.GameState.SecondExamPassed)
            {
                _skitSystemManager.SetTestSkitId("01_SecondExam2");
            }
            else if (_testSkitFlagData.CurrentGameState == TestSkitFlagData.GameState.FirstExamFailed)
            {
                _skitSystemManager.SetTestSkitId("01_FirstExam3");
            }
   

            await _skitSystemManager.Initialize();
            _loadingPanel.FadeOutAsync();
            _skitSystemManager.DoSkitSequence().Forget();
        }

        private void Start()
        {
            CRIAudioManager.BGM.Stop();
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));
        }
    }
}
