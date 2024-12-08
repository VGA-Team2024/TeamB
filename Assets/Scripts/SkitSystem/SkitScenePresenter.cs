using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using TGS2023.BGM;

namespace TeamB.SkitSystem
{
    public class SkitScenePresenter : MonoBehaviour
    {
        private enum DataLoadType
        {
            Remote,
            Local
        }

        [SerializeField] private SkitView _skitSceneView;
        [SerializeField] private SkitResourceLoader _skitResourceLoader;
        [SerializeField] private SkitViewFade _loadingPanel;
        [SerializeField] private SkitFlagData _skitFlagData;
        [SerializeField] private SkitLogViewer _skitLogViewer;
        [SerializeField] private DataLoadType _dataLoadType = DataLoadType.Remote;
        public SkitSystemManager SkitSystemManager { get; private set; }
        public ISkitDataLoader SkitDataLoader { get; private set; }
        public SkitFlagData SkitFlagData => _skitFlagData;

        private async void Awake()
        {
            _loadingPanel.FadeInAsync(true).Forget();
            if (_dataLoadType == DataLoadType.Remote)
            {
                // リモートからデータをロード
                SkitDataLoader = new RemoteSkitDataLoader();
                await SkitDataLoader.InitTalkData();
            }
            else
            {
                // TODO:ローカルからデータをロード
            }

            await _skitResourceLoader.InitializeSkitResourceLoader();
            _skitSceneView.InitializeSkitView(_skitResourceLoader);
            SetSkitDataHandler();
            SkitSystemManager.DoSkitSequence().Forget();
        }

        /// <summary>
        /// SkitDataHandlerを生成し、SkitSystemManagerに登録する
        /// </summary>
        private void SetSkitDataHandler()
        {
            var classSelectSkitContextHandler = new ClassSelectSkitContextHandler(SkitDataLoader);
            var skitDataHandler = new SkitDataHandler(SkitDataLoader);
            var tutorialHandler = new TutorialHandler(SkitDataLoader);
            var skitContextHandlers = new HashSet<SkitContextHandlerBase>
            {
                classSelectSkitContextHandler,
                skitDataHandler,
                tutorialHandler,
            };
            var skitSceneCoordinator = new TestSkitSceneCoordinator(SkitDataLoader, _skitFlagData);
            SkitSystemManager = new SkitSystemManager(skitContextHandlers, skitSceneCoordinator);

            // SkitDataHandlerとViewの紐付け
            var skitDataHandlerDisposable = skitDataHandler.CurrentSkitEntryData.Subscribe(skitEntryData =>
            {
                if (skitEntryData == null) return;
                if (skitEntryData is SkitChoiceData skitChoiceData)
                {
                    _skitSceneView.ShowSkitChoice(skitChoiceData, skitDataHandler.AwaitForSelect,
                        skitDataHandler.AwaitForEmptyInput, skitChoiceData.ChoiceTime,
                        SkitSystemManager.CurrentCancellationToken.Token).Forget();
                }
                else
                {
                    _skitSceneView.ShowSkit(skitEntryData, skitDataHandler.AwaitForEmptyInput,
                        SkitSystemManager.CurrentCancellationToken.Token).Forget();
                }
                _skitLogViewer.SetLog(skitEntryData);
            }).AddTo(_skitSceneView);
            
            var classSelectSkitContextHandlerDisposable = classSelectSkitContextHandler.CurrentClassSelectData
                .Subscribe(result =>
                {
                    if (result == null) return;
                    _skitSceneView.ShowClassSelect(result, classSelectSkitContextHandler.AwaitForSelect,
                        SkitSystemManager.CurrentCancellationToken.Token).Forget();
                }).AddTo(_skitSceneView);
            
            var tutorialAboutGameDisposable = tutorialHandler.TutorialDataAboutGame.Subscribe(result =>
            {
                if (result == null) return;
                _skitSceneView.ShowTutorialAboutGame(result, tutorialHandler.AwaitForEmptyInput,
                    SkitSystemManager.CurrentCancellationToken.Token).Forget();
            }).AddTo(_skitSceneView);
            
            var tutorialAboutSkitChoiceDisposable = tutorialHandler.TutorialChoiceData.Subscribe(result =>
            {
                if (result == null) return;
                _skitSceneView.ShowTutorialAboutSkitChoice(result, tutorialHandler.AwaitForSelect,
                    SkitSystemManager.CurrentCancellationToken.Token);
            }).AddTo(_skitSceneView);
            
            var tutorialAboutClassSelectDisposable = tutorialHandler.TutorialClassSelectData.Subscribe(result =>
            {
                if (result == null) return;
                _skitSceneView.ShowTutorialAboutClassSelect(result, tutorialHandler.AwaitForSelect,
                    SkitSystemManager.CurrentCancellationToken.Token);
            }).AddTo(_skitSceneView);
            
            var tutorialAboutSkitResultDisposable = tutorialHandler.TutorialDataAboutSkitChoiceResult.Subscribe(result =>
            {
                if (result == null) return;
                _skitSceneView.ShowTutorialAboutSkitResult(result, tutorialHandler.AwaitForEmptyInput,
                    SkitSystemManager.CurrentCancellationToken.Token).Forget();
            }).AddTo(_skitSceneView);

            SkitSystemManager.CurrentCancellationToken?.Token.Register(() =>
            {
                classSelectSkitContextHandlerDisposable.Dispose();
                skitDataHandlerDisposable.Dispose();
                tutorialAboutGameDisposable.Dispose();
                tutorialAboutSkitChoiceDisposable.Dispose();
                tutorialAboutClassSelectDisposable.Dispose();
                tutorialAboutSkitResultDisposable.Dispose();
            });
        }

    private void Start()
        {
            CRIAudioManager.BGM.Stop();
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));
        }

        private void OnDestroy()
        {
            SkitSystemManager?.Dispose();
        }
    }
}
