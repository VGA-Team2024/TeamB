using System;
using System.Collections;
using System.Collections.Generic;
using DataManagement.SpreadSheet;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using TeamB.InGameData.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamB.Develop
{
    /// <summary>
    /// 敵側を管理するクラス
    /// </summary>
    public class EnemyManager : MonoBehaviour, IExam
    {
        #region serializeFields

        [SerializeReference, SubclassSelector] private IEnemy _currentEnemy;

        #endregion

        #region privates

        private WaveManager _waveManager;
        private AllyManager _allyManager;
        private Exam _exam;

        #endregion

        #region properties

        public IEnemy GetCurrentEnemyData => _currentEnemy;

        #endregion

        private void Awake()
        {
            Initialized();
        }


        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialized()
        {
            _exam = FindAnyObjectByType<Exam>();
            _waveManager = FindAnyObjectByType<WaveManager>();
            _allyManager = FindAnyObjectByType<AllyManager>();
            if (_exam)
            {
                _exam.OnExamStarted += OnStartExam;
                _exam.OnExamEnded += OnEndExam;
            }
        }


        /// <summary>
        /// 全敵の攻撃処理
        /// </summary>
        /// <param name="deltaTime"></param>
        private void EnemiesAttack(float deltaTime)
        {
            _currentEnemy.Attack(_allyManager.GetAllies, _exam.GetOperationType, deltaTime);
        }
        

        /// <summary>
        /// 形態変化時
        /// </summary>
        private void OnNextForm()
        {
            _waveManager.NextWave();
        }

        /// <summary>
        /// 敵が全滅しているか
        /// </summary>
        private bool IsAnnihilation() => _currentEnemy.GetCurrentData.Hp <= 0;

        /// <summary>
        /// 戦闘開始時処理
        /// </summary>
        public void OnStartExam()
        {
            _currentEnemy.Initialized();
            _currentEnemy.OnDeath += _exam.ExamClear;
            _currentEnemy.OnNextForm += OnNextForm;
            _exam.OnExamUpdated += EnemiesAttack;
        }

        /// <summary>
        /// 戦闘終了時処理
        /// </summary>
        public void OnEndExam()
        {
            _exam.OnExamStarted -= OnStartExam;
            _exam.OnExamEnded -= OnEndExam;
            _exam.OnExamUpdated -= EnemiesAttack;
            _currentEnemy.Dispose();
        }
    }
}