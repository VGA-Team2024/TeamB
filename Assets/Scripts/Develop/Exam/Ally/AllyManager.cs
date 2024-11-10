using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// 試験シーンの味方を管理するクラス
    /// </summary>
    public class AllyManager : MonoBehaviour, IExam
    {
        [SerializeReference, SubclassSelector] private IAlly _allies;

        private EnemyManager _enemyManager;
        private Exam _exam;
        DefenseInput _defenseInput = new();
        AttackInput _attackInput = new();

        public IAlly GetAllies => _allies;

        private void Awake()
        {
            Initialized();
        }

        private void Initialized()
        {
            _exam = FindAnyObjectByType<Exam>();
            _enemyManager = FindAnyObjectByType<EnemyManager>();

            _exam.OnExamStarted += OnStartExam;
            _exam.OnExamEnded += OnEndExam;
        }


        /// <summary>
        /// 味方の攻撃呼び出し
        /// </summary>
        /// <param name="deltaTime"></param>
        private void AlliesAttack(float deltaTime)
        {
            _allies.Attack(_enemyManager.GetCurrentEnemyData, _exam.GetOperationType, deltaTime);
        }

        /// <summary>
        /// 味方の防御呼び出し
        /// </summary>
        /// <param name="deltaTime"></param>
        private void AlliesDefense(float deltaTime)
        {
            _allies.Defense(_exam.GetOperationType, deltaTime);
        }

        /// <summary>
        /// 試験開始の処理
        /// </summary>
        public void OnStartExam()
        {
            _exam.OnExamUpdated += OnUpdateExam;
            _allies.Initialized();
            _allies.OnDeath += () => { _exam.EndExam(); };
        }

        /// <summary>
        /// 試験中の処理
        /// </summary>
        private void OnUpdateExam(float deltaTime)
        {
            //入力受付
            _attackInput.ChangeInput(Input.GetKeyDown(KeyCode.Space));
            _defenseInput.ChangeInput(Input.GetKeyDown(KeyCode.Space));
            _allies.Input(_attackInput, _defenseInput);

            //攻撃の呼び出し
            AlliesAttack(deltaTime);
            //防御の呼び出し
            AlliesDefense(deltaTime);
        }

        /// <summary>
        /// 試験終了処理
        /// </summary>
        public void OnEndExam()
        {
            _exam.OnExamStarted -= OnStartExam;
            _exam.OnExamEnded -= OnEndExam;
            _allies.Dispose();
        }
    }

    /// <summary>
    /// 主人公の行動の種類（攻撃、防御）
    /// </summary>
    public enum ActionType
    {
        Attack,
        Defend,
        None
    }


    /// <summary>
    /// 攻撃の入力管理クラス
    /// </summary>
    public class AttackInput : IInputType
    {
        private bool _isInput;
        public bool IsInput => _isInput;

        public void ChangeInput(bool value)
        {
            _isInput = value;
        }
    }

    /// <summary>
    /// 防御の入力管理クラス
    /// </summary>
    public class DefenseInput : IInputType
    {
        private bool _isInput;
        public bool IsInput => _isInput;

        public void ChangeInput(bool value)
        {
            _isInput = value;
        }
    }
}