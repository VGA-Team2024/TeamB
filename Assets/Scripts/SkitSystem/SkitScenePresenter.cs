using System.Collections.Generic;
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
        private SkitSystemManager _skitSystemManager;
        public ISkitDataLoader SkitDataLoader;

        private async void Awake()
        {
            _loadingPanel.gameObject.SetActive(true);
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
            var classSelectSkitContextHandler = new ClassSelectSkitContextHandler(SkitDataLoader);
            var skitDataHandler = new SkitDataHandler(SkitDataLoader);
            var skitChoiceHandler = new SkitChoiceHandler(SkitDataLoader);
            var skitContextHandlers = new HashSet<SkitContextHandlerBase>
            {
                classSelectSkitContextHandler,
                skitDataHandler,
                skitChoiceHandler
            };
            var skitSceneCoordinator = new TestSkitSceneCoordinator(SkitDataLoader, _skitFlagData);
            _skitSystemManager = new SkitSystemManager(skitContextHandlers, skitSceneCoordinator);
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
                    //Todo:選択肢入力の際にクリックで進んでしまう問題を解決する
                    skitDataHandler.AwaitForInput?.TrySetResult(("", ""));
                }).AddTo(this);
            await _loadingPanel.FadeOutAsync();
            _skitSystemManager.DoSkitSequence().Forget();
        }

        private void Start()
        {
            CRIAudioManager.BGM.Stop();
            CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_002_InGame));
        }
    }
}
