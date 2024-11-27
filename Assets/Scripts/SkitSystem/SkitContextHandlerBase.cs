using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Debug = UnityEngine.Debug;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 渡される会話シーンでのデータを処理する基底クラス
    /// </summary>
    public abstract class SkitContextHandlerBase : IDisposable
    {
        public abstract SkitContext.ContextType HandleSkitContextType { get; }
        protected ISkitDataLoader _skitDataLoader;
        public UniTaskCompletionSource AwaitForEmptyInput { get; protected set; }
        public UniTaskCompletionSource<string> AwaitForSelect { get; protected set; }
        public abstract UniTask HandleSkitContext(SkitContext skitContext, CancellationToken token);
        public abstract bool TrtGetNextSkitContext(out SkitContext nextSkitContext);

        protected SkitContextHandlerBase(ISkitDataLoader skitDataLoader)
        {
            _skitDataLoader = skitDataLoader;
        }

        protected virtual void OnDispose()
        {
        }

        public void Dispose()
        {
            AwaitForEmptyInput?.TrySetCanceled();
            AwaitForSelect?.TrySetCanceled();
            OnDispose();
        }
    }

    public class ClassSelectSkitContextHandler : SkitContextHandlerBase
    {
        private SkitContext _nextSkitContext;
        public override SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.ClassSelect;
        
        private readonly ReactiveProperty<ClassSelectData> _currentClassSelectData = new();
        public ReadOnlyReactiveProperty<ClassSelectData> CurrentClassSelectData => _currentClassSelectData;

        public ClassSelectSkitContextHandler(ISkitDataLoader skitDataLoader) : base(skitDataLoader)
        {
        }

        public override bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }

        public override async UniTask HandleSkitContext(SkitContext skitContext, CancellationToken token)
        {
            AwaitForSelect = new UniTaskCompletionSource<string>();
            if (skitContext.SkitSceneData is not ClassSelectData classSelectData)
            {
                Debug.LogError("ClassSelectDataが見つかりませんでした");
                return;
            }
            _currentClassSelectData.Value = classSelectData;
            var result = await AwaitForSelect.Task;
            token.ThrowIfCancellationRequested();
            //選択した選択肢に対応するデータを取得
            if (_skitDataLoader.TryGetSkitData(result, out var skitData))
            {
                _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, skitData);
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

        public override async UniTask HandleSkitContext(SkitContext skitContext, CancellationToken token)
        {
            if (skitContext.SkitSceneData is not SkitData skitData)
            {
                Debug.LogError("SkitDataが見つかりませんでした");
                return;
            }

            foreach (var skitEntryData in skitData.SkitEntryData)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.Log("処理がキャンセルされました");
                    return;
                }

                //ここでタグの読み取りなど行う。次に表示するデータ等を含んでいたら_nextSkitContextに代入する
                //ToDo: タグの読み取りの処理の分離
                var normDialogue = skitEntryData.JapaneseTalkDialogue;
                if (skitEntryData.JapaneseTalkDialogue.Contains("[MainCharacter]"))
                {
                    normDialogue = normDialogue.Replace("[MainCharacter]", "リアン");
                }

                if (skitEntryData.JapaneseTalkDialogue.Contains("[Skit]"))
                {
                    normDialogue = normDialogue.Replace("[Skit]", "");
                    var match = Regex.Match(normDialogue, @"\[(.*?)\]");
                    if (!match.Success) return;
                    var skitId = match.Groups[1].Value;
                    normDialogue = normDialogue.Replace($"[{skitId}]", "");
                    if (_skitDataLoader.TryGetSkitData(skitId, out var nextSkitData))
                    {
                        _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, nextSkitData);
                    }
                    else
                    {
                        Debug.LogError("SkitDataが見つかりませんでした");
                    }
                }

                AwaitForEmptyInput = new UniTaskCompletionSource();
                AwaitForSelect = new UniTaskCompletionSource<string>();
                if (skitEntryData.JapaneseTalkDialogue.Contains("[Choice]"))
                {
                    var dialogue = normDialogue.Replace("[Choice]", "");
                    var match = Regex.Match(dialogue, @"\[(.*?)\]");
                    if (!match.Success) return;
                    var skitId = match.Groups[1].Value;
                    if (_skitDataLoader.TryGetSkitChoiceData(skitId, out var choiceData))
                    {
                        var currentSkitChoiceData = new SkitChoiceData(choiceData.Id, choiceData.ChoiceTime,
                            choiceData.Answer, choiceData.ChoiceEntries, choiceData.JapaneseTalkDialogue,
                            skitEntryData.TalkCharaData,
                            skitEntryData.TalkSpeaker, skitEntryData.TalkBackground, choiceData.EnglishTalkDialogue);
                        _currentSkitEntryData.Value = currentSkitChoiceData;
                    }
                    else
                    {
                        Debug.LogError("ChoiceDataが見つかりませんでした");
                    }

                    var start = DateTime.Now;
                    var result = await AwaitForSelect.Task.AttachExternalCancellation(token);
                    if (choiceData.Answer == result)
                    {
                        var end = DateTime.Now;
                        var time = end - start;
                        SkitRewardManager.Instance.AddRewardValue(choiceData.ChoiceTime - (float)time.TotalSeconds);
                    }
                }
                else
                {
                    var currentSkitEntryData = new SkitEntryData(skitEntryData.TalkCharaData, skitEntryData.TalkSpeaker,
                        skitEntryData.TalkBackground, normDialogue, skitEntryData.EnglishTalkDialogue);
                    _currentSkitEntryData.Value = currentSkitEntryData;
                }

                await AwaitForEmptyInput.Task.AttachExternalCancellation(token);
                if (token.IsCancellationRequested)
                {
                    Debug.Log("処理がキャンセルされました");
                    return;
                }
            }
        }

        public override bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = _nextSkitContext;
            _nextSkitContext = null;
            return nextSkitContext != null;
        }
    }
}