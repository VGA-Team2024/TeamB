using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// 試験シーンの味方を管理するクラス
    /// </summary>
    public class AllyManager : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private ICharacter _allies;

        private EnemyManager _enemyManager;
        private Exam _exam;

        public ICharacter GetAllies => _allies;

        private void Awake()
        {
            _exam = FindAnyObjectByType<Exam>();
            Initialized();
        }

        private void Initialized()
        {
            _enemyManager = FindAnyObjectByType<EnemyManager>();

            _exam.OnExamStarted += OnExamStarted;
            _exam.OnExamEnded += OnExamEnded;
        }

        /// <summary>
        /// 試験開始時処理
        /// </summary>
        private void OnExamStarted()
        {
            _exam.OnExamUpdated += AlliesAttack;
            _allies.Initialized();
            _allies.OnDeath += () =>
            {
                _exam.EndExam();
            };

        }

        /// <summary>
        /// 試験終了時処理
        /// </summary>
        private void OnExamEnded()
        {
            _exam.OnExamStarted -= OnExamStarted;
            _exam.OnExamEnded -= OnExamEnded;
            _exam.OnExamUpdated -= AlliesAttack;
            _allies.Dispose();
        }

        /// <summary>
        /// 味方の攻撃処理
        /// </summary>
        /// <param name="deltaTime"></param>
        private void AlliesAttack(float deltaTime)
        {
            _allies.Attack(_enemyManager.GetCurrentEnemyData, deltaTime);
        }
    }
}