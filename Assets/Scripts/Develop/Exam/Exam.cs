using System;
using System.Collections.Generic;
using System.Linq;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using TeamB.SkitSystem;
using UnityEngine;
using UnityEngine.Playables;
using CsvLoader = TeamB.GameSystem.CsvLoader;

namespace TeamB.Develop
{
    /// <summary>
    /// 試験シーン全体を管理するクラス
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class Exam : MonoBehaviour
    {
        [SerializeField] private float _examTime = 45f;
        [SerializeField] private OperationType _operationType;
        [SerializeField] private PlayableDirector _winDirector;

        [SerializeField] private ExamStateDatas _examStateDatas;
        [SerializeField] private SkitFlagData _skitFlagData;


        private string _examDataURL =
            "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=1160587194&single=true&output=csv";

        private PoseManager _poseManager;
        private float _currentTimer = 0f;
        public event Action OnExamStarted;
        public event Action<float> OnExamUpdated;
        public event Action OnExamEnded;

        public OperationType GetOperationType => _operationType;
        public float GetCurrentTimer => _currentTimer;
        public float GetMaxTime => _examTime;

        private void Awake()
        {
            StartExam();
            InitialExamData();
        }

        private void Update()
        {
            if (_poseManager == null)
                _poseManager = FindObjectOfType<PoseManager>();
            if (_poseManager && !_poseManager.GetIsInPose)
            {
                OnExamUpdated?.Invoke(Time.deltaTime);
            }
        }

        /// <summary>
        /// 試験の開始
        /// </summary>
        private void StartExam()
        {
            _poseManager = FindObjectOfType<PoseManager>();
            OnExamUpdated += Timer;
            OnExamStarted?.Invoke();
        }

        /// <summary>
        /// 試験の終了
        /// </summary>
        public void EndExam()
        {
            OnExamEnded?.Invoke();
        }

        /// <summary>
        /// 操作方法の変更
        /// </summary>
        public void ChangeOperation()
        {
            _operationType = _operationType == OperationType.Auto ? OperationType.Manual : OperationType.Auto;
        }

        /// <summary>
        /// 時間管理
        /// </summary>
        /// <param name="deltaTime"></param>
        private void Timer(float deltaTime)
        {
            if (_currentTimer >= _examTime)
            {
                switch (GameStatics.ExamState)
                {
                    case ExamState.FirstExam:
                        ExamClear();
                        break;
                    case ExamState.SecondExam:
                        ExamFailure();
                        break;
                }

                _currentTimer = 0f;
            }
            else
            {
                _currentTimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// 試験クリア
        /// </summary>
        public void ExamClear()
        {
            GameStatics.ExamResult = ExamResult.Clear;
            EndExam();
            OnStartPose();
            _winDirector.Play();

            string flagName = String.Empty;
            switch (GameStatics.ExamState)
            {
                case ExamState.FirstExam:
                    flagName = _examStateDatas.Data.First(x => x.CurrentState == nameof(ExamState.FirstExam))
                        .ClearState;
                    _skitFlagData.SetCurrentFlag(flagName);
                    GameStatics.ExamState = ExamState.SecondExam;
                    break;
                case ExamState.SecondExam:
                    flagName = _examStateDatas.Data.First(x => x.CurrentState == nameof(ExamState.SecondExam))
                        .ClearState;
                    _skitFlagData.SetCurrentFlag(flagName);
                    GameStatics.ExamState = ExamState.ExamClear;
                    break;
            }
        }

        /// <summary>
        /// 試験失敗
        /// </summary>
        public void ExamFailure()
        {
            GameStatics.ExamResult = ExamResult.Failed;
            EndExam();
            OnStartPose();
            SceneLoader.LoadScene("Result");

            string flagName = String.Empty;
            switch (GameStatics.ExamState)
            {
                case ExamState.FirstExam:
                    flagName = _examStateDatas.Data.First(x => x.CurrentState == nameof(ExamState.FirstExam))
                        .FailureState;
                    _skitFlagData.SetCurrentFlag(flagName);
                    break;
                case ExamState.SecondExam:
                    flagName = _examStateDatas.Data.First(x => x.CurrentState == nameof(ExamState.SecondExam))
                        .FailureState;
                    _skitFlagData.SetCurrentFlag(flagName);
                    break;
            }
        }

        public void OnEndPose()
        {
            if (_poseManager == null)
                _poseManager = FindAnyObjectByType<PoseManager>();
            _poseManager.StopPose();
        }

        public void OnStartPose()
        {
            if (_poseManager == null)
                _poseManager = FindAnyObjectByType<PoseManager>();
            if (_poseManager != null)
                _poseManager.StartPose();
        }

        private async void InitialExamData()
        {
            List<string[]> rawData = await CsvLoader.GetSpreadsheetDataAsync(_examDataURL);
            List<ExamStateData> examData = new List<ExamStateData>();

            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }

            for (var i = 1; i < rawData.Count; i++)
            {
                var data = rawData[i];

                var classChoiceData = new ExamStateData
                {
                    CurrentState = data[0],
                    ClearState = data[1],
                    FailureState = data[2],
                };
                examData.Add(classChoiceData);
            }

            _examStateDatas.Data = examData;
        }
    }

    /// <summary>
    /// Managerクラスでスタートとエンドをすぐ作る用
    /// </summary>
    interface IExam
    {
        public void OnStartExam();
        public void OnEndExam();
    }

    /// <summary>
    /// 操作方法の種類（自動、手動）
    /// </summary>
    public enum OperationType
    {
        Auto,
        Manual,
        None
    }
}