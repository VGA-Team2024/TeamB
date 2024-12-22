using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// 敵側を管理するクラス
    /// </summary>
    public class EnemyManager : MonoBehaviour, IExam, IPoseObject
    {
        #region serializeFields

        [SerializeReference, SubclassSelector] private IEnemy _currentEnemy;
        [SerializeField] private GameObject _enemy;

        #endregion

        #region privates

        private WaveManager _waveManager;
        private AllyManager _allyManager;
        private PoseManager _poseManager;
        private Exam _exam;
        List<SpriteRenderer> _spriteRenderers = new();

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
            _poseManager = FindAnyObjectByType<PoseManager>();
            _currentEnemy.OnTakeDamage += OnTakeDamage;
            if (_exam)
            {
                _exam.OnExamStarted += OnStartExam;
                _exam.OnExamEnded += OnEndExam;
            }

            _exam.OnExamUpdated += (_) =>
            {
                if (_poseManager == null)
                {
                    _poseManager = FindAnyObjectByType<PoseManager>();
                    _poseManager.OnInPose += StartPose;
                    _poseManager.OnOutPose += EndPose;
                }
            };

            foreach (var sprite in _enemy.GetComponentsInChildren<SpriteRenderer>())
            {
                _spriteRenderers.Add(sprite);
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

        private async void OnTakeDamage()
        {
            _spriteRenderers.Select(x => x.color = new Color(1f, 0f, 0f, 1f));
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            if(!_enemy)return;
            
            _spriteRenderers.Select(x => x.color = new Color(1f, 1f, 1f, 1f));
            if(_currentEnemy.GetCurrentCondition == AbnormalCondition.Stunned)
                _spriteRenderers.Select(x => x.color = new Color(1f, 1f, 0f, 1f));
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

        public void StartPose()
        {
            _currentEnemy.StartPose();
        }

        public void EndPose()
        {
            _currentEnemy.EndPose();
        }
    }
}