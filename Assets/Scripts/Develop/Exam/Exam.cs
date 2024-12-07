using System;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.Playables;

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
                ExamFailure();
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
        }

        public void OnEndPose()
        {
            if(_poseManager == null)
                _poseManager = FindAnyObjectByType<PoseManager>();
            _poseManager.StopPose();
        }
        public void OnStartPose()
        {
            if(_poseManager == null)
                _poseManager = FindAnyObjectByType<PoseManager>();
            _poseManager.StartPose();
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
