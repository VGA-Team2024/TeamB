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
        [SerializeField] private SkitContext.ContextType _testSkitContextType = SkitContext.ContextType.Skit;
        [SerializeField] private TestSkitFlagData _testSkitFlagData;
        private readonly Queue<SkitContext> _skitContextQueue = new();
        private readonly HashSet<ISkitContextHandler> _skitContextHandlers = new();
        private ISkitDataLoader _skitDataLoader;
        public ISkitDataLoader SkitDataLoader => _skitDataLoader;
        
        public void SetTestSkitSceneData(string testSkitId, SkitContext.ContextType testSkitContextType)
        {
            _testSkitId = testSkitId;
            _testSkitContextType = testSkitContextType;
        }
        
        public void SetSkitContextHandlers(ISkitContextHandler skitContextHandler)
        {
            _skitContextHandlers.Add(skitContextHandler);
        }
        
        public async UniTask Initialize()
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

            switch (_testSkitContextType)
            {
                case SkitContext.ContextType.Skit:
                    if (_skitDataLoader.TryGetSkitData(_testSkitId, out var skitData))
                    {
                        _skitContextQueue.Enqueue(new SkitContext(SkitContext.ContextType.Skit, skitData));
                    }
                    break;
                case SkitContext.ContextType.ClassSelect:
                    if (_skitDataLoader.TryGetClassSelectData(_testSkitId, out var classSelectData))
                    {
                        _skitContextQueue.Enqueue(new SkitContext(SkitContext.ContextType.ClassSelect, classSelectData));
                    }
                    break;
                case SkitContext.ContextType.SkitChoice:
                    if (_skitDataLoader.TryGetSkitChoiceData(_testSkitId, out var skitChoiceData))
                    {
                        _skitContextQueue.Enqueue(new SkitContext(SkitContext.ContextType.SkitChoice, skitChoiceData));
                    }
                    break;
            }
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
                        await skitContextHandler.HandleSkitContext(_skitContextQueue.Dequeue(), _skitDataLoader);

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

            if (_testSkitFlagData.CurrentGameState == TestSkitFlagData.GameState.SecondExamPassed)
            {
                SceneLoader.LoadScene("Title");
            }
            else
            {
                // 会話データがなくなったらシーン遷移
                SceneLoader.LoadScene("Exam");
            }
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
