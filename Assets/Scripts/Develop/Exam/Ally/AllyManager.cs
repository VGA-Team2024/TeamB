using System;
using Cysharp.Threading.Tasks;
using TeamB.GameSystem;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// 試験シーンの味方を管理するクラス
    /// </summary>
    public class AllyManager : MonoBehaviour, IExam
    {
        [SerializeReference, SubclassSelector] private IAlly _allies;
        [SerializeField] GameObject alliesPrefab;
        [SerializeField] GameObject defencePrefab;

        private EnemyManager _enemyManager;
        private Exam _exam;
        private BuffContainer _buffContainer;
        private PoseManager _poseManager;
        private DefenseInput _defenseInput = new();
        private AttackInput _attackInput = new();

        public IAlly GetAllies => _allies;

        private void Awake()
        {
            Initialized();
        }

        private void Initialized()
        {
            _exam = FindAnyObjectByType<Exam>();
            _enemyManager = FindAnyObjectByType<EnemyManager>();
            _buffContainer = FindAnyObjectByType<BuffContainer>();

            _exam.OnExamStarted += OnStartExam;
            _exam.OnExamEnded += OnEndExam;
            _allies.OnTakeDamage += OnTakeDamage;
            _allies.OnDefense += OnDefense;
            _allies.OnEndDefense += OnEndDefense;
        }

        private async void OnTakeDamage()
        {
            foreach (var sprite in alliesPrefab.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(1, 0, 0, 1);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            if (!alliesPrefab) return;
            foreach (var sprite in alliesPrefab.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(1f, 1, 1, 1);
            }
        }

        private void OnDefense()
        {
            defencePrefab.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 1);
            defencePrefab.SetActive(true);
        }

        private void OnEndDefense()
        {
            defencePrefab.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 1);
            defencePrefab.SetActive(false);
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
        /// バフ追加の呼び出し
        /// </summary>
        /// <param name="buffType"></param>
        public void AddBuff(BuffType buffType)
        {
            if (_poseManager == null)
                _poseManager = FindAnyObjectByType<PoseManager>();
            if (!_poseManager.GetIsInPose)
                _allies.AddBuff(_buffContainer.GetBuffData((int)buffType));
        }

        /// <summary>
        /// バフ解除の呼び出し（持続時間が過ぎたら消える）
        /// </summary>
        /// <param name="deltaTime"></param>
        private void RemoveBuff(float deltaTime)
        {
            _allies.RemoveBuff(deltaTime);
        }

        /// <summary>
        /// デバフ追加の呼び出し
        /// </summary>
        /// <param name="buffType"></param>
        public void AddDeBuff(BuffType buffType)
        {
            if (_poseManager == null)
                _poseManager = FindAnyObjectByType<PoseManager>();
            if (!_poseManager.GetIsInPose)
                _allies.AddDeBuff(_buffContainer.GetBuffData((int)buffType));
        }

        /// <summary>
        /// デバフ解除の呼び出し（持続時間が過ぎたら消える）
        /// </summary>
        /// <param name="deltaTime"></param>
        private void RemoveDeBuff(float deltaTime)
        {
            _allies.RemoveDeBuff(deltaTime);
        }

        /// <summary>
        /// 試験開始の処理
        /// </summary>
        public void OnStartExam()
        {
            _exam.OnExamUpdated += OnUpdateExam;
            _allies.Initialized();
            _allies.OnDeath += _exam.ExamFailure;
        }

        /// <summary>
        /// 試験中の処理
        /// </summary>
        private void OnUpdateExam(float deltaTime)
        {
            //入力受付
            _allies.Input(_attackInput, _defenseInput);

            //攻撃の呼び出し
            AlliesAttack(deltaTime);
            //防御の呼び出し
            AlliesDefense(deltaTime);
            //バフ解除の呼び出し
            RemoveBuff(deltaTime);
            //デバフ解除の呼び出し
            RemoveDeBuff(deltaTime);
        }

        public void Action(bool isAction)
        {
            _attackInput.ChangeInput(isAction);
            _defenseInput.ChangeInput(isAction);
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