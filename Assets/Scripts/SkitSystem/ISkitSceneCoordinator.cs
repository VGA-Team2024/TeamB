using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.Develop.Develop;
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
        public void EndSkitScene();
    }
    
    public class TestSkitSceneCoordinator : ISkitSceneCoordinator
    {
        private readonly ISkitDataLoader _skitDataLoader;
        private readonly SkitFlagData _skitFlagData;
        public TestSkitSceneCoordinator(ISkitDataLoader skitDataLoader, SkitFlagData skitFlagData)
        {
            _skitDataLoader = skitDataLoader;
            _skitFlagData = skitFlagData;
        }

        public SkitContext GetStartSkitData()
        {
            if (_skitDataLoader.TryGetSkitSceneDataByFlag(_skitFlagData, out var skitSceneData))
            {
                Debug.Log(skitSceneData.Id);
                switch (skitSceneData)
                {
                    case ClassSelectData _:
                        return new SkitContext(SkitContext.ContextType.ClassSelect, skitSceneData, _skitFlagData);
                    case SkitData _:
                        return new SkitContext(SkitContext.ContextType.Skit, skitSceneData, _skitFlagData);
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        public void EndSkitScene()
        {
            // 会話シーンの終了時に必要な処理を行う
            SceneLoader.LoadScene(_skitFlagData.CurrentFlag == "SecondExamClear" ? "Title" : "Exam");
            //SceneLoader.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
