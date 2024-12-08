using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーンの進行を処理するクラス
    /// </summary>
    public class SkitSystemManager : IDisposable
    {
        private readonly Queue<SkitContext> _skitContextQueue = new();
        private readonly HashSet<SkitContextHandlerBase> _skitContextHandlers = new();
        private readonly ISkitSceneCoordinator _skitSceneCoordinator;
        private bool _isFirstSkitContextExecuted = false;
        public CancellationTokenSource CurrentCancellationToken { get; private set; }

        
        public SkitSystemManager(HashSet<SkitContextHandlerBase> skitContextHandlers, ISkitSceneCoordinator skitSceneCoordinator)
        {
            _skitContextHandlers.UnionWith(skitContextHandlers);
            _skitSceneCoordinator = skitSceneCoordinator;
            _skitContextQueue.Enqueue(_skitSceneCoordinator.GetStartSkitData());
        }
        
        public void ResetSkitSceneData()
        {
            _skitContextQueue.Clear();
        }
        
        public void SetSkitSceneData(SkitContext testSkitContext)
        {
            _skitContextQueue.Enqueue(testSkitContext);
        }

        public async UniTask DoSkitSequence()
        {
            CancelSkitSequence();
            while (_skitContextQueue.Count > 0)
            {
                var currentSkitContext = _skitContextQueue.Peek();
                if (currentSkitContext == null)
                {
                    Debug.LogError("SkitContextがnullです");
                    _skitContextQueue.Dequeue(); // null要素を削除してスキップ
                    continue;
                }

                var handleSkitContextType = currentSkitContext.SkitContextType;

                // ハンドラを取得
                var validHandlers = _skitContextHandlers
                    .Where(handler => handler.HandleSkitContextType == handleSkitContextType)
                    .ToList();

                if (!validHandlers.Any())
                {
                    Debug.LogError($"SkitContextType {handleSkitContextType} に対応するハンドラが見つかりません");
                    _skitContextQueue.Dequeue(); // 対応するハンドラがない場合はスキップ
                    continue;
                }
                foreach (var skitContextHandler in validHandlers)
                {
                    // 現在のコンテキストを処理し、デキュー
                    await skitContextHandler.HandleSkitContext(_skitContextQueue.Dequeue(),
                        CurrentCancellationToken.Token);

                    // 次のスキットコンテキストがある場合、エンキュー
                    if (skitContextHandler.TrtGetNextSkitContext(out var nextSkitContext))
                    {
                        _skitContextQueue.Enqueue(nextSkitContext);
                    }
                }
            }
            
            //テスト用
            SkitRewardManager.Instance.ApplyStatus();
            _skitSceneCoordinator.EndSkitScene();
        }
        private void CancelSkitSequence()
        {
            CurrentCancellationToken?.Cancel();
            _skitContextHandlers.ToList().ForEach(handler => handler.Dispose());
            CurrentCancellationToken = new CancellationTokenSource();
        }
        
        public void Dispose()
        {
            CancelSkitSequence();
        }
    }
    
    
    
    public class SkitContext
    {
        public enum ContextType
        {
            ClassSelect,
            Skit,
            Tutorial,
        }
        
        public SkitContext(ContextType skitContextType, ISkitSceneData skitSceneData, SkitFlagData skitFlagData)
        {
            SkitContextType = skitContextType;
            SkitSceneData = skitSceneData;
            SkitFlagData = skitFlagData;
        }

        public readonly SkitFlagData SkitFlagData;
        public readonly ContextType SkitContextType;
        public readonly ISkitSceneData SkitSceneData;
    }
}
