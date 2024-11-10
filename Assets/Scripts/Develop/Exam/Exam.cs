using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.Threading;
using DataManagement;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamB.Develop
{
    /// <summary>
    /// 試験シーン全体を管理するクラス
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class Exam : MonoBehaviour
    {
        [SerializeField] private float _examTime = 45f;
        [SerializeField]　private OperationType _operationType;
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
            OnExamUpdated?.Invoke(Time.deltaTime);
        }

        /// <summary>
        /// 試験の開始
        /// </summary>
        private void StartExam()
        {
            OnExamUpdated += Timer;
            OnExamStarted?.Invoke();
        }

        /// <summary>
        /// 試験の終了
        /// </summary>
        public void EndExam()
        {
            OnExamEnded?.Invoke();
            DebugManager.Log("Exam Ended");
            SceneLoader.LoadScene("Result");
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
                EndExam();
                _currentTimer = 0f;
            }
            else
            {
                _currentTimer += Time.deltaTime;
            }
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