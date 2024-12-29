using System;
using Cysharp.Threading.Tasks;
using VOICE.Lian;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using TeamB.UI;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// 試験シーンの味方を管理するクラス
    /// </summary>
    public class AllyManager : MonoBehaviour, IExam, IPoseObject
    {
        [SerializeReference, SubclassSelector] private IAlly _allies;
        [SerializeField] private ParticleSystem _attackBuffParticles;
        [SerializeField] private ParticleSystem _DefenceBuffParticles;
        [SerializeField] private GameObject alliesPrefab;
        [SerializeField] private GameObject _defencePrefab;


        private EnemyManager _enemyManager;
        private Exam _exam;
        private BuffContainer _buffContainer;
        private PoseManager _poseManager;
        private DefenseInput _defenseInput = new();
        private AttackInput _attackInput = new();

        public event Action OnEndDamageEffect;

        private int _defenceSuccessCount;
        private int _attackSuccessCount;
        private int _hitCount;

        public IAlly GetAllies => _allies;
        public int GetDefenceSuccessCount => _defenceSuccessCount;
        public int GetAttackSuccessCount => _attackSuccessCount;
        public int GetHitCunt => _hitCount;

        private async void Awake()
        {
            Initialized();
        }

        private void Initialized()
        {
            _exam = FindAnyObjectByType<Exam>();
            _enemyManager = FindAnyObjectByType<EnemyManager>();
            _buffContainer = FindAnyObjectByType<BuffContainer>();
            _poseManager = FindAnyObjectByType<PoseManager>();

            _exam.OnExamStarted += OnStartExam;
            _exam.OnExamEnded += OnEndExam;
            _allies.OnTakeDamage += OnTakeDamage;
            _allies.OnDefense += OnDefense;
            _allies.OnEndDefense += OnEndDefense;
            _allies.OnEndAttack += OnSuccessAttack;
            _allies.OnDefenceFailure += DefenceFailure;
            _exam.OnExamUpdated += (_) =>
            {
                if (_poseManager == null)
                {
                    _poseManager = FindAnyObjectByType<PoseManager>();
                    _poseManager.OnInPose += StartPose;
                    _poseManager.OnOutPose += EndPose;
                }
            };
        }

        private async void OnTakeDamage()
        {
            if (GameStatics.GetRandomNumber(2) == 0)
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(VOICE.Lian.Lian.Lian_15));
            }
            else
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(VOICE.Lian.Lian.Lian_16));
            }

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

            OnEndDamageEffect?.Invoke();
        }

        private void OnDefense()
        {
            if (!_defencePrefab)
                return;
            _defencePrefab.SetActive(true);
            _allies.OnSuccessDefence += OnSuccessDefence;
        }

        private void OnEndDefense()
        {
            if (!_defencePrefab)
                return;
            _defencePrefab.SetActive(false);
            _allies.OnSuccessDefence -= OnSuccessDefence;
        }

        private void OnSuccessDefence()
        {
            foreach (var sprite in alliesPrefab.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(1, 0.6f, 0, 1);
            }

            CRIAudioManager.VOICE.Play("Lian", nameof(VOICE.Lian.Lian.Lian_12));
            _defenceSuccessCount++;
        }

        private void DefenceFailure()
        {
            _hitCount++;
        }

        private void OnSuccessAttack()
        {
            OnAttackCountUp();
            FindObjectOfType<InExamUIView>().ScoreChange();
        }

        public void OnAttackCountUp()
        {
            _attackSuccessCount++;
        }

        /// <summary>
        /// 味方の攻撃呼び出し
        /// </summary>
        /// <param name="deltaTime"></param>
        private async void AlliesAttack(float deltaTime)
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
            _allies.AddBuff(_buffContainer.GetBuffData((int)buffType));
            switch (buffType)
            {
                case BuffType.Attack:
                    _attackBuffParticles.Play();
                    break;
                case BuffType.De_GiveDamage:
                    _DefenceBuffParticles.Play();
                    break;
            }
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
            if (!_exam)
                return;
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

        public void AttackAction(bool isAction)
        {
            _attackInput.ChangeInput(isAction);
        }

        public void DefenceAction(bool isAction)
        {
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

        public void StartPose()
        {
            _allies.StartPose();
        }

        public void EndPose()
        {
            _allies.EndPose();
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