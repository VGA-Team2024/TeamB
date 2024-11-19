using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 渡される会話シーンでのデータを処理するインターフェース
    /// </summary>
    public interface ISkitContextHandler
    {
        public bool TrtGetNextSkitContext(out SkitContext nextSkitContext);
        public SkitContext.ContextType HandleSkitContextType { get; }
        public UniTask HandleSkitContext(SkitContext skitContext, ISkitDataLoader skitDataLoader);
        public UniTaskCompletionSource<(string choiceId, string result)> AwaitForNextUts { get; }
    }
    
    public class ClassSelectSkitContextHandler : ISkitContextHandler
    {
        private SkitContext _nextSkitContext;
        public bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }

        public SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.ClassSelect;
        public UniTaskCompletionSource<(string choiceId, string result)> AwaitForNextUts { get; private set; } 

        public async UniTask HandleSkitContext(SkitContext skitContext, ISkitDataLoader skitDataLoader)
        {
            AwaitForNextUts = new UniTaskCompletionSource<(string choiceId, string result)>();
            var result = await AwaitForNextUts.Task;
            //選択した選択肢に対応するデータを取得
            if (skitDataLoader.TryGetSkitData(result.choiceId, out var classSelectData))
            {
                _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, classSelectData);
            }
            else
            {
                Debug.LogError("ClassSelectDataが見つかりませんでした");
            }
        }
    }
    
    public class SkitDataHandler : ISkitContextHandler
    {
        private SkitContext _nextSkitContext;
        public SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.Skit;

        public UniTaskCompletionSource<(string choiceId, string result)> AwaitForNextUts { get; private set; }
        private readonly ReactiveProperty<SkitEntryData> _currentSkitEntryData = new();
        public ReadOnlyReactiveProperty<SkitEntryData> CurrentSkitEntryData => _currentSkitEntryData;
        public async UniTask HandleSkitContext(SkitContext skitContext, ISkitDataLoader skitDataLoader)
        {
            
            if (skitContext.SkitSceneData is not SkitData skitData)
            {
                Debug.LogError("SkitDataが見つかりませんでした");
                return;
            }

            foreach (var skitEntryData in skitData.SkitEntryData)
            {
                //ここでタグの読み取りなど行う。次に表示するデータ等を含んでいたら_nextSkitContextに代入する
                //ToDo: タグの読み取りの処理の分離
                if (skitEntryData.JapaneseTalkDialogue.Contains("[MainCharacter]"))
                {
                    skitEntryData.JapaneseTalkDialogue = skitEntryData.JapaneseTalkDialogue.Replace("[MainCharacter]", "リアン");
                }
                
                if (skitEntryData.JapaneseTalkDialogue.Contains("[Skit]"))
                {
                    skitEntryData.JapaneseTalkDialogue = skitEntryData.JapaneseTalkDialogue.Replace("[Skit]", "");
                    var match = Regex.Match(skitEntryData.JapaneseTalkDialogue, @"\[(.*?)\]");
                    if (!match.Success) return;
                    var skitId = match.Groups[1].Value;
                    skitEntryData.JapaneseTalkDialogue = skitEntryData.JapaneseTalkDialogue.Replace($"[{skitId}]", "");
                    if (skitDataLoader.TryGetSkitData(skitId, out var nextSkitData))
                    {
                        _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, nextSkitData);
                    }
                    else
                    {
                        Debug.LogError("SkitDataが見つかりませんでした");
                    }
                }
                
                _currentSkitEntryData.Value = skitEntryData;
                AwaitForNextUts = new UniTaskCompletionSource<(string choiceId, string result)>();
                await AwaitForNextUts.Task;
            }
        }
        public bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }
    }
    
    public class SkitChoiceHandler : ISkitContextHandler
    {
        private SkitContext _nextSkitContext;
        public bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }

        public SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.SkitChoice;
        public UniTaskCompletionSource<(string choiceId, string result)> AwaitForNextUts { get; private set; }

        public async UniTask HandleSkitContext(SkitContext skitContext, ISkitDataLoader skitDataLoader)
        {
            if (skitContext.SkitSceneData is not SkitChoiceData skitChoiceData)
            {
                Debug.LogError("SkitChoiceDataが見つかりませんでした");
                return;
            }

            AwaitForNextUts = new UniTaskCompletionSource<(string choiceId, string result)>();
            var resultTask = AwaitForNextUts.Task;
            var delayTask = UniTask.WaitForSeconds(skitChoiceData.ChoiceTime);
            var startTime = Time.realtimeSinceStartup;
            var waitResult = await UniTask.WhenAny(resultTask, delayTask);
            if (waitResult.hasResultLeft && waitResult.result.choiceId == skitChoiceData.Answer)
            {
                var elapsedTime = Time.realtimeSinceStartup - startTime;
                var remainingTime = skitChoiceData.ChoiceTime - elapsedTime;
                SkitRewardManager.Instance.AddRewardValue(remainingTime);
            }
            //もし選択肢データが次の会話データの内容をもっていたら_nextSkitContextに代入する
        }
    }
}
