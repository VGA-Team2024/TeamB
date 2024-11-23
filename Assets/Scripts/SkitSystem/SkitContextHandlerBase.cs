using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 渡される会話シーンでのデータを処理するインターフェース
    /// </summary>
    public abstract class SkitContextHandlerBase
    {
        public abstract SkitContext.ContextType HandleSkitContextType { get; }
        protected ISkitDataLoader _skitDataLoader;
        public UniTaskCompletionSource<(string choiceId, string result)> AwaitForInput { get; protected set; }
        public abstract UniTask HandleSkitContext(SkitContext skitContext);
        public abstract bool TrtGetNextSkitContext(out SkitContext nextSkitContext);
        
        protected SkitContextHandlerBase(ISkitDataLoader skitDataLoader)
        {
            _skitDataLoader = skitDataLoader;
        }
    }
    
    public class ClassSelectSkitContextHandler : SkitContextHandlerBase
    {
        private SkitContext _nextSkitContext;
        public override SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.ClassSelect;
        
        public ClassSelectSkitContextHandler(ISkitDataLoader skitDataLoader) : base(skitDataLoader)
        {
        }
        public override bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }

        public override async UniTask HandleSkitContext(SkitContext skitContext)
        {
            AwaitForInput = new UniTaskCompletionSource<(string choiceId, string result)>();
            var result = await AwaitForInput.Task;
            //選択した選択肢に対応するデータを取得
            if (_skitDataLoader.TryGetSkitData(result.choiceId, out var classSelectData))
            {
                _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, classSelectData);
            }
            else
            {
                Debug.LogError("ClassSelectDataが見つかりませんでした");
            }
        }

    }
    
    public class SkitDataHandler : SkitContextHandlerBase
    {
        private SkitContext _nextSkitContext;
        public override SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.Skit;

        private readonly ReactiveProperty<SkitEntryData> _currentSkitEntryData = new();
        public ReadOnlyReactiveProperty<SkitEntryData> CurrentSkitEntryData => _currentSkitEntryData;
        
        public SkitDataHandler(ISkitDataLoader skitDataLoader) : base(skitDataLoader)
        {
        }
        public  override async UniTask HandleSkitContext(SkitContext skitContext)
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
                    if (_skitDataLoader.TryGetSkitData(skitId, out var nextSkitData))
                    {
                        _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, nextSkitData);
                    }
                    else
                    {
                        Debug.LogError("SkitDataが見つかりませんでした");
                    }
                }
                
                _currentSkitEntryData.Value = skitEntryData;
       
                AwaitForInput = new UniTaskCompletionSource<(string choiceId, string result)>();
                await AwaitForInput.Task;
            }
        }
        public override bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }

    }
    
    public class SkitChoiceHandler : SkitContextHandlerBase
    {
        private SkitContext _nextSkitContext;
        public override SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.SkitChoice;
        
        public SkitChoiceHandler(ISkitDataLoader skitDataLoader) : base(skitDataLoader)
        {
        }
        public override bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }

        public override async UniTask HandleSkitContext(SkitContext skitContext)
        {
            if (skitContext.SkitSceneData is not SkitChoiceData skitChoiceData)
            {
                Debug.LogError("SkitChoiceDataが見つかりませんでした");
                return;
            }

            AwaitForInput = new UniTaskCompletionSource<(string choiceId, string result)>();
            var resultTask = AwaitForInput.Task;
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
