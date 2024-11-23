using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーンの進行を処理するクラス
    /// </summary>
    public class SkitSystemManager
    {
        private readonly Queue<SkitContext> _skitContextQueue = new();
        private readonly HashSet<SkitContextHandlerBase> _skitContextHandlers = new();
        private readonly ISkitSceneCoordinator _skitSceneCoordinator;

        public SkitSystemManager(HashSet<SkitContextHandlerBase> skitContextHandlers, ISkitSceneCoordinator skitSceneCoordinator)
        {
            _skitContextHandlers.UnionWith(skitContextHandlers);
            _skitSceneCoordinator = skitSceneCoordinator;
            _skitContextQueue.Enqueue(_skitSceneCoordinator.GetStartSkitData());
        }
        
        public void SetSkitSceneData(SkitContext testSkitContext)
        {
            _skitContextQueue.Enqueue(testSkitContext);
        }

        public async UniTask DoSkitSequence()
        {
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
                    try
                    {
                        // 現在のコンテキストを処理し、デキュー
                        await skitContextHandler.HandleSkitContext(_skitContextQueue.Dequeue());

                        // 次のスキットコンテキストがある場合、エンキュー
                        if (skitContextHandler.TrtGetNextSkitContext(out var nextSkitContext))
                        {
                            _skitContextQueue.Enqueue(nextSkitContext);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log("処理がキャンセルされました");
                        return; // キャンセルされた場合は処理を終了
                    }
                }
            }
            
            //テスト用
            _skitSceneCoordinator.EndSkitScene();
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
