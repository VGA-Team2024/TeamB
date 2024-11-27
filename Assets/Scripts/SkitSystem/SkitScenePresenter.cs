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
        [SerializeField] private DataLoadType _dataLoadType = DataLoadType.Remote;
        public SkitSystemManager SkitSystemManager { get; private set; }
        public ISkitDataLoader SkitDataLoader { get; private set; }

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
            await _loadingPanel.FadeOutAsync();
        }

        /// <summary>
        /// SkitDataHandlerを生成し、SkitSystemManagerに登録する
        /// </summary>
        private void SetSkitDataHandler()
        {
            var classSelectSkitContextHandler = new ClassSelectSkitContextHandler(SkitDataLoader);
            var skitDataHandler = new SkitDataHandler(SkitDataLoader);
            var skitContextHandlers = new HashSet<SkitContextHandlerBase>
            {
                classSelectSkitContextHandler,
                skitDataHandler,
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
                    _skitSceneView.ShowSkit(skitEntryData, skitDataHandler.AwaitForEmptyInput, SkitSystemManager.CurrentCancellationToken.Token).Forget();
                }
            }).AddTo(_skitSceneView);
            SkitSystemManager.CurrentCancellationToken?.Token.Register(() =>
            {
                skitDataHandlerDisposable.Dispose();
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
