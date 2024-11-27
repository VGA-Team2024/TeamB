using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            SkitData skitSceneData = null;
            switch (_skitFlagData.CurrentGameState)
            {
                // 会話シーンの開始時に必要なデータを取得
                case SkitFlagData.GameState.Prologue:
                case SkitFlagData.GameState.FirstExam:
                {
                    if (_skitDataLoader.TryGetSkitData("01_prologue1", out skitSceneData))
                    {
                        _skitFlagData.CurrentGameState = SkitFlagData.GameState.FirstExam;
                    }
                    break;
                }
                case SkitFlagData.GameState.FirstExamPassed:
                {
                    if (_skitDataLoader.TryGetSkitData("01_FirstExam2", out skitSceneData))
                    {
                        _skitFlagData.CurrentGameState = SkitFlagData.GameState.SecondExam;
                    }

                    break;
                }
                case SkitFlagData.GameState.FirstExamFailed:
                {
                    if (_skitDataLoader.TryGetSkitData("01_FirstExam3", out skitSceneData))
                    {
                        _skitFlagData.CurrentGameState = SkitFlagData.GameState.SecondExam;
                    }

                    break;
                }
                case SkitFlagData.GameState.SecondExamPassed:
                {
                    if (_skitDataLoader.TryGetSkitData("01_SecondExam2", out skitSceneData))
                    {
                        _skitFlagData.CurrentGameState = SkitFlagData.GameState.SecondExam;
                    }

                    break;
                }
                case SkitFlagData.GameState.SecondExamFailed:
                {
                    if (_skitDataLoader.TryGetSkitData("01_SecondExam3", out skitSceneData))
                    {
                        _skitFlagData.CurrentGameState = SkitFlagData.GameState.SecondExam;
                    }

                    break;
                }
                case SkitFlagData.GameState.SecondExam:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new SkitContext(SkitContext.ContextType.Skit, skitSceneData);
        }

        public void EndSkitScene()
        {
            // 会話シーンの終了時に必要な処理を行う
            SceneLoader.LoadScene(_skitFlagData.CurrentGameState == SkitFlagData.GameState.SecondExamPassed ? "Title" : "Exam");
        }
    }
}
