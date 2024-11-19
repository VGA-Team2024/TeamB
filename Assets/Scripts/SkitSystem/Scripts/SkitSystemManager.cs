using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーン全体を管理するクラス
    /// </summary>
    public class SkitSystemManager : MonoBehaviour
    {
        private enum DataLoadType
        {
            Remote,
            Local
        }
        
        [SerializeField] private DataLoadType _dataLoadType;
        [SerializeField] private string _testSkitId = "01_prologue1";
        private readonly Queue<SkitContext> _skitContextQueue = new();
        private readonly HashSet<ISkitContextHandler> _skitContextHandlers = new();
        private ISkitDataLoader _skitDataLoader;
        
        private async void Start()
        {
            await Initialize();
            DoSkitSequence().Forget();
        }
        
        public void SetSkitContextHandlers(ISkitContextHandler skitContextHandler)
        {
            _skitContextHandlers.Add(skitContextHandler);
        }
        
        private async UniTask Initialize()
        {
            if (_dataLoadType == DataLoadType.Remote)
            {
                // リモートからデータをロード
                _skitDataLoader = new RemoteSkitDataLoader();
                await _skitDataLoader.InitTalkData();
            }
            else
            {
                // TODO:ローカルからデータをロード
            }
            
            if (_skitDataLoader.TryGetSkitData(_testSkitId, out var skitData))
            {
                _skitContextQueue.Enqueue(new SkitContext(SkitContext.ContextType.Skit, skitData));
            }
            else
            {
                Debug.LogError("テスト用のSkitDataが見つかりませんでした");
            }
        }

        private async UniTask DoSkitSequence()
        {
            while (_skitContextQueue.Count > 0)
            {
                var currentSkitContext = _skitContextQueue.Peek();
                if (currentSkitContext == null)
                {
                    Debug.LogError("SkitContextがnullです");
                    _skitContextQueue.Dequeue(); // null要素をスキップしてキューから削除
                    break;
                }
                // 表示する会話データの内容に応じて処理を行う
                var handleSkitContextType = _skitContextQueue.Peek().SkitContextType;
                foreach (var skitContextHandler in _skitContextHandlers.Where(skitContextHandler => skitContextHandler.HandleSkitContextType == handleSkitContextType))
                {
                    try
                    {
                        await skitContextHandler.HandleSkitContext(_skitContextQueue.Dequeue(), _skitDataLoader);
                        if (skitContextHandler.TrtGetNextSkitContext(out var nextSkitContext))
                        {
                            _skitContextQueue.Enqueue(nextSkitContext);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log("処理がキャンセルされました");
                        return; // キャンセルされた場合は処理を抜ける
                    }
                }
            }

            // 会話データがなくなったらシーン遷移
            SceneLoader.LoadScene("Exam");
        }
    }
    
    
    public class SkitContext
    {
        public enum ContextType
        {
            ClassSelect,
            Skit,
            SkitChoice,
        }
        
        public SkitContext(ContextType skitContextType, ISkitSceneData skitSceneData)
        {
            SkitContextType = skitContextType;
            SkitSceneData = skitSceneData;
        }
 
        public readonly ContextType SkitContextType;
        public readonly ISkitSceneData SkitSceneData;
    }
}
