using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity.VisualScripting;
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
            if (_skitDataLoader.TryGetSkitDataById(result, out var skitData))
            {
                _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, skitData, skitContext.SkitFlagData);
            }
            else
            {
                Debug.LogError($"ClassSelectData : {result}が見つかりませんでした");
            }
        }
    }

    public class SkitDataHandler : SkitContextHandlerBase
    {
        private SkitContext _nextSkitContext;
        public override SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.Skit;

        private readonly ReactiveProperty<SkitEntryData> _currentSkitEntryData = new();
        public ReadOnlyReactiveProperty<SkitEntryData> CurrentSkitEntryData => _currentSkitEntryData;

        public SkitDataHandler(ISkitDataLoader skitDataLoader) : base(skitDataLoader) { }

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
                
                if (skitEntryData.JapaneseTalkDialogue.Contains("SetFlag"))
                {
                    var pattern = @"\[SetFlag:(.*?)\]";
                    var match = Regex.Match(normDialogue, pattern);
                    var flagValue = match.Success ? match.Groups[1].Value : null;
                    skitContext.SkitFlagData.SetCurrentFlag(flagValue);
                    normDialogue = Regex.Replace(normDialogue, pattern, "");
                }
      

                if (skitEntryData.JapaneseTalkDialogue.Contains("[Skit]"))
                {
                    normDialogue = normDialogue.Replace("[Skit]", "");
                    var match = Regex.Match(normDialogue, @"\[(.*?)\]");
                    if (!match.Success) return;
                    var skitId = match.Groups[1].Value;
                    normDialogue = normDialogue.Replace($"[{skitId}]", "");
                    if (_skitDataLoader.TryGetSkitDataById(skitId, out var nextSkitData))
                    {
                        _nextSkitContext = new SkitContext(SkitContext.ContextType.Skit, nextSkitData, skitContext.SkitFlagData);
                    }
                    else
                    {
                        Debug.LogError($"SkitData : {skitId} が見つかりませんでした");
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
                    if (_skitDataLoader.TryGetSkitChoiceDataByID(skitId, out var choiceData))
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

    public class TutorialHandler : SkitContextHandlerBase
    {
        private readonly ReactiveProperty<NormalTutorialData> _tutorialDataAboutGame = new();
        public ReadOnlyReactiveProperty<NormalTutorialData> TutorialDataAboutGame => _tutorialDataAboutGame;
        private readonly ReactiveProperty<TutorialChoiceData> _tutorialChoiceData = new();
        public ReadOnlyReactiveProperty<TutorialChoiceData> TutorialChoiceData => _tutorialChoiceData;
        private readonly ReactiveProperty<TutorialClassSelectData> _tutorialClassSelectData = new();
        public ReadOnlyReactiveProperty<TutorialClassSelectData> TutorialClassSelectData => _tutorialClassSelectData;
        private readonly ReactiveProperty<NormalTutorialData> _tutorialDataAboutSkitChoiceResult = new();
        public ReadOnlyReactiveProperty<NormalTutorialData> TutorialDataAboutSkitChoiceResult => _tutorialDataAboutSkitChoiceResult;

        public TutorialHandler(ISkitDataLoader skitDataLoader) : base(skitDataLoader)
        {
        }

        public override SkitContext.ContextType HandleSkitContextType => SkitContext.ContextType.Tutorial;

        public override async UniTask HandleSkitContext(SkitContext skitContext, CancellationToken token)
        {
            if (skitContext.SkitSceneData is not TutorialData tutorialData)
            {
                Debug.LogError("TutorialDataが見つかりませんでした");
                return;
            }

            for (var index = 0; index < tutorialData.JapaneseDialogue.Length; index++)
            {
                var dialog = tutorialData.JapaneseDialogue[index];
                Debug.Log(dialog);
                if (token.IsCancellationRequested)
                {
                    Debug.Log("処理がキャンセルされました");
                    return;
                }

                AwaitForEmptyInput = new UniTaskCompletionSource();
                AwaitForSelect = new UniTaskCompletionSource<string>();
                var normDialogue = dialog.Trim();
                if (normDialogue.Contains("ClassSelect"))
                {
                    var match = Regex.Match(normDialogue, @"\[ClassSelect:(.+?)\]");
                    if (!match.Success) return;
                    var skitId = match.Groups[1].Value;
                    if (_skitDataLoader.TryGetClassSelectDataById(skitId, out var classSelectData))
                    {
                        _tutorialClassSelectData.Value = new TutorialClassSelectData(classSelectData.Id,
                            classSelectData.Flag, classSelectData.TalkerName, classSelectData.BackgroundImageName,
                            classSelectData.Dialogue, classSelectData.ClassChoices, normDialogue);
                    }
                    else
                    {
                        Debug.LogError("ClassSelectDataが見つかりませんでした");
                    }
                    await AwaitForSelect.Task.AttachExternalCancellation(token);
                }
                else if (normDialogue.Contains("Choice"))
                {
                    Debug.Log("Choice");
                    var match = Regex.Match(normDialogue, @"\[Choice:(.+?)\]");
                    if (!match.Success) return;
                    var skitId = match.Groups[1].Value;
                    if (_skitDataLoader.TryGetSkitChoiceDataByID(skitId, out var choiceData))
                    {
                        _tutorialChoiceData.Value = new TutorialChoiceData(choiceData.Id, choiceData.ChoiceTime,
                            choiceData.Answer, choiceData.ChoiceEntries, choiceData.JapaneseTalkDialogue,
                            choiceData.TalkCharaData, choiceData.TalkSpeaker, choiceData.EnglishTalkDialogue,
                            choiceData.TalkBackground,
                            normDialogue);
                    }
                    else
                    {
                        Debug.LogError("ChoiceDataが見つかりませんでした");
                    }
                    await AwaitForSelect.Task.AttachExternalCancellation(token);
                }
                else
                {
                    var currentTutorialData = new NormalTutorialData(normDialogue, tutorialData.BackgroundImageName);
                    if (index == tutorialData.JapaneseDialogue.Length - 1)
                    {
                        _tutorialDataAboutSkitChoiceResult.Value = currentTutorialData;
                    }
                    else
                    {
                        _tutorialDataAboutGame.Value = currentTutorialData;
                    }
                    await AwaitForEmptyInput.Task.AttachExternalCancellation(token);
                }
            }

        }

        public override bool TrtGetNextSkitContext(out SkitContext nextSkitContext)
        {
            nextSkitContext = null;
            return false;
        }
    }
}