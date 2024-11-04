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
    public class EnemyManager : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private IEnemy _currentEnemy;
        [SerializeField] private float _addSoul = 10;
        
        private WaveManager _waveManager;
        private AllyManager _allyManager;
        private SoulManager _soulManager;
        private Exam _exam;

        public IEnemy GetCurrentEnemyData => _currentEnemy;

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
            _soulManager = FindAnyObjectByType<SoulManager>();
            if (_exam)
            {
                _exam.OnExamStarted += OnExamStart;
                _exam.OnExamEnded += OnExamEnded;
            }
        }
        

        /// <summary>
        /// 全敵の攻撃処理
        /// </summary>
        /// <param name="deltaTime"></param>
        private void EnemiesAttack(float deltaTime)
        {
            _currentEnemy.Attack(_allyManager.GetAllies, deltaTime);
        }

        /// <summary>
        /// 試験開始
        /// </summary>
        private void OnExamStart()
        {
            _currentEnemy.Initialized();
            _currentEnemy.OnDeath += OnEnemyDeath;
            _currentEnemy.OnNextForm += OnNextForm;
            _exam.OnExamUpdated += EnemiesAttack;
        }

        /// <summary>
        /// 試験終了時処理
        /// </summary>
        private void OnExamEnded()
        {
            _exam.OnExamStarted -= OnExamStart;
            _exam.OnExamEnded -= OnExamEnded;
            _exam.OnExamUpdated -= EnemiesAttack;
            _currentEnemy.Dispose();
        }

        /// <summary>
        /// 生徒会長が負けた時
        /// </summary>
        private void OnEnemyDeath()
        {
            DebugManager.Log($"{_currentEnemy.GetCurrentData.Card}に勝った");
            GameStatics.ExamState = ExamState.SecondExam;
            _exam.EndExam();
        }

        /// <summary>
        /// 形態変化時
        /// </summary>
        private void OnNextForm()
        {
            _waveManager.NextWave();
            _soulManager.AddSoul(_addSoul);
        }


        /// <summary>
        /// 敵が全滅しているか
        /// </summary>
        private bool IsAnnihilation() => _currentEnemy.GetCurrentData.Hp <= 0;
    }
}