using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 開始時のSkitContextの取得と終了時の処理を行うインターフェース
    /// </summary>
    public interface ISkitSceneCoordinator
    {
        public SkitContext GetStartSkitData(); 
        public UniTask EndSkitScene(CancellationToken cancellationToken);
    }
    
    public class SkitSceneCoordinator : ISkitSceneCoordinator
    {
        private readonly SkitDataLoaderBase _skitDataLoaderBase;
        private readonly SkitFlagData _skitFlagData;
        private const string TitleSceneName = "Title";
        private const string ExamSceneName = "Exam";
        private const string LastFlag = "SecondExamClear";
        private const string DefaultId = "Prologue";
        private const float VoiceDelay = 6;
        private readonly NextLoadScene _nextLoadScene;
        public enum NextLoadScene
        {
            Skit,
            Exam
        }
        
        public SkitSceneCoordinator(SkitDataLoaderBase skitDataLoaderBase, SkitFlagData skitFlagData, NextLoadScene nextLoadScene)
        {
            _skitDataLoaderBase = skitDataLoaderBase;
            _skitFlagData = skitFlagData;
            _nextLoadScene = nextLoadScene;
        }

        public SkitContext GetStartSkitData()
        {
            if (_skitDataLoaderBase.TryGetSkitSceneDataByFlag(_skitFlagData, out var skitSceneData))
            {
                switch (skitSceneData)
                {
                    case ClassSelectData _:
                        return new SkitContext(SkitContext.ContextType.ClassSelect, skitSceneData, _skitFlagData);
                    case SkitData _:
                        return new SkitContext(SkitContext.ContextType.Skit, skitSceneData, _skitFlagData);
                }
            }
            Debug.LogError("SkitSceneDataがnullです: " + _skitFlagData.CurrentFlag);
            _skitDataLoaderBase.TryGetSkitDataById(DefaultId, out var defaultSkitSceneData);
            {
                return new SkitContext(SkitContext.ContextType.Skit, defaultSkitSceneData, _skitFlagData);
            }
        }

        public async UniTask EndSkitScene(CancellationToken cancellationToken)
        {
            CRIAudioManager.VOICE.Play(SkitSoundHelper.LianSheetName,
                SkitRewardManager.Instance.IsParameterUp
                    ? SkitSoundHelper.VoiceParameterUp
                    : SkitSoundHelper.VoiceAfterClass);
            await UniTask.WaitForSeconds(VoiceDelay, cancellationToken: cancellationToken);
            // 会話シーンの終了時に必要な処理を行う
            if (_nextLoadScene == NextLoadScene.Skit)
            {
                SceneLoader.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                if (_skitFlagData.CurrentFlag == LastFlag)
                {
                    GameEventRecorder.GameEnd(() => SceneLoader.LoadScene(TitleSceneName));
                }
                else
                {
                    SceneLoader.LoadScene(ExamSceneName);
                }
            }
        }
    }
}
