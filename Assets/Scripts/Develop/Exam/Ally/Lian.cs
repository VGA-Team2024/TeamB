using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DataManagement.SpreadSheet;
using SE.Lian;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace TeamB.Develop
{
    /// <summary>
    ///     キャラクターを管理するクラス : サンプルクラス
    ///     このように実装すれば量産できるよ
    /// </summary>
    public class Lian : IAlly
    {
        #region SerializedFields

        [SerializeField] private Transform _attackParticleTrans;

        [SerializeField] private float _percentageReductionValue = 0.75f;

        [SerializeField] private float _defenceCoolTime = 1f;

        [SerializeField] private float _defenceDuration = 1f;

        #endregion

        #region Privates

        private ICharacter _character;

        private CancellationToken _token = new CancellationToken();
        private AsyncOperationHandle<GameObject> _handle;
        private List<ParticleSystem> _particles = new();
        private float _percentageReduction;
        private float _defenceDurationTimer;
        private readonly float _percentageReductionBaseValue = 1;

        private bool _isAttacking;
        private bool _isDefending;
        private bool _isDefenceDuration;

        #endregion

        #region Actions

        public event Action OnDeath;
        public event Action OnAttack;
        public event Action OnEndAttack;
        public event Action OnDefense;
        public event Action OnEndDefense;
        public event Action OnSuccessDefence;
        public event Action OnDefenceFailure;
        public event Action OnTakeDamage;
        public event Action OnTakeHeal;
        public event Action OnAddBuff;
        public event Action OnRemoveBuff;
        public event Action OnAddDeBuff;
        public event Action OnRemoveDeBuff;
        public event Action OnParamUpDate;

        #endregion

        #region Properties

        // 戦闘用のパラメータ
        /// <summary> 戦闘用のパラメータ </summary>
        public CharacterData GetCurrentData { get; private set; }

        public List<IBuff> GetHaveBuffs { get; } = new();

        public List<IBuff> GetHaveDeBuffs { get; } = new();

        // キャラの種類
        public CharacterType GetFirstCharacterType => GameStatics.NurturingCharacterType;

        public ActionType GetActionType { get; private set; }

        public float GetAttackCoolTimer { get; private set; }

        public float GetDefenceCoolTimer { get; private set; }

        public float GetDefenceCoolTime => _defenceCoolTime;

        #endregion

        public void Initialized()
        {
            _handle = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Effect2/Particle_Battle_Attack01.prefab");
            GetCurrentData = new CharacterData(GameStatics.Characters[(int)GameStatics.NurturingCharacterType]);
            _token = new CancellationTokenSource().Token;
            _percentageReduction = _percentageReductionBaseValue;


            OnParamUpDate?.Invoke();
        }

        /// <summary>
        ///     キャラの登録処理
        /// </summary>
        /// <param name="type"></param>
        public void RegistrationType(CharacterType type)
        {
            GameStatics.NurturingCharacterType = type;
            GetCurrentData = new CharacterData(GameStatics.Characters[(int)GameStatics.NurturingCharacterType]);
        }

        /// <summary>
        ///     攻撃処理
        /// </summary>
        public async void Attack<T>(T characters, OperationType operationType, float deltaTime) where T : ICharacter
        {
            if (GetAttackCoolTimer >= TakeBuff(BuffType.CastingSpeed, GetCurrentData.ChantingSpeed))
            {
                //操作方法が自動時、ゲームプレイヤーの入力を待つ
                if (operationType == OperationType.Manual && !_isAttacking)
                    return;

                _character = characters;

                GameObject attackParticle = GameObject.Instantiate(_handle.Result, _attackParticleTrans.position,
                    _attackParticleTrans.rotation);
                ParticleSystem attackParticleSystem = attackParticle.GetComponent<ParticleSystem>();
                attackParticleSystem.Play();
                _particles.Add(attackParticleSystem);

                int randVoice = Random.Range(0, 2);
                if (randVoice == 0)
                {
                    CRIAudioManager.VOICE.Play("Lian", nameof(SE.Lian.Lian.Lian_10));
                }
                else
                {
                    CRIAudioManager.VOICE.Play("Lian", nameof(SE.Lian.Lian.Lian_11));
                }

                attackParticleSystem.Play();
                float rand = Random.Range(0, 100);
                if (rand <= TakeBuff(BuffType.HitRate, GetCurrentData.HitRate))
                {
                    OnAttack?.Invoke();

                    GetAttackCoolTimer = 0;

                    ParticleCallBack particleCallBack = attackParticle.GetComponent<ParticleCallBack>();
                    particleCallBack.OnCallBack += GiveDamage;
                    particleCallBack.OnCallBack += () =>
                    {
                        _particles.Remove(attackParticleSystem);
                        Object.Destroy(attackParticle);
                    };

                    OnEndAttack?.Invoke();
                }
                else
                {
                    GetAttackCoolTimer = 0;
                    DebugManager.Log("主人公の攻撃が外れた");
                }
            }
            else
            {
                GetAttackCoolTimer += deltaTime;
            }
        }

        public void AttackCancel()
        {
        }

        /// <summary>
        ///     攻撃を受ける際の処理
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            if (GetCurrentData.Hp <= 0)
                return;

            //HPの更新
            if (damage >= 0)
            {
                GetCurrentData.Hp -= damage * _percentageReduction;
                OnTakeDamage?.Invoke();
                if (_isDefenceDuration)
                {
                    OnSuccessDefence?.Invoke();
                }
                else
                {
                    OnDefenceFailure?.Invoke();
                }
            }
            else
            {
                GetCurrentData.Hp -= damage;
                if (GetCurrentData.Hp > GameStatics.Characters[(int)GameStatics.NurturingCharacterType].Hp)
                {
                    GetCurrentData.Hp = GameStatics.Characters[(int)GameStatics.NurturingCharacterType].Hp;
                }

                OnTakeHeal?.Invoke();
            }

            //死亡時処理
            if (GetCurrentData.Hp <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        /// <summary>
        ///     バフ追加
        /// </summary>
        /// <param name="buff"></param>
        public void AddBuff(IBuff buff)
        {
            GetHaveBuffs.Add(buff);
            OnAddBuff?.Invoke();
        }

        /// <summary>
        ///     バフ解除
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RemoveBuff(float deltaTime)
        {
            for (var i = 0; i < GetHaveBuffs.Count; i++)
            {
                if (GetHaveBuffs[i] != null && GetHaveBuffs[i].Timer(deltaTime))
                {
                    GetHaveBuffs.RemoveAt(i);
                    OnRemoveBuff?.Invoke();
                    i--;
                }
            }
        }

        /// <summary>
        ///     デバフ追加
        /// </summary>
        /// <param name="buff"></param>
        public void AddDeBuff(IBuff buff)
        {
            GetHaveDeBuffs.Add(buff);
            OnAddDeBuff?.Invoke();
        }

        /// <summary>
        ///     デバフ解除
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RemoveDeBuff(float deltaTime)
        {
            for (var i = 0; i < GetHaveDeBuffs.Count; i++)
                if (GetHaveDeBuffs[i].Timer(deltaTime))
                {
                    GetHaveDeBuffs.RemoveAt(i);

                    OnRemoveDeBuff?.Invoke();
                    i--;
                }
        }

        /// <summary>
        ///     バフ、デバフの適用
        /// </summary>
        /// <param name="buffType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public float TakeBuff(BuffType buffType, float value)
        {
            var buffs = GetHaveBuffs.Where(x => x.GetBuffType == buffType).ToList();
            if (buffs.Count == 0)
                return value;

            var buffed = value;

            foreach (var buff in buffs)
            {
                //加算バフ、デバフ
                if (buff.GetCalculationMethod == CalculationMethod.Addition &&
                    buff.GetBuffType == buffType)
                    buffed += buff.GetValue;
                //乗算バフ、デバフ
                if (buff.GetCalculationMethod == CalculationMethod.Multiplication &&
                    buff.GetBuffType == buffType)
                    buffed *= buff.GetValue;
            }

            return buffed;
        }


        public void Dispose()
        {
            OnAttack = default;
            OnEndAttack = default;
            OnDefense = default;
            OnEndDefense = default;
            OnParamUpDate = default;
            OnTakeDamage = default;
            OnDeath = default;
            _handle.Release();
        }

        /// <summary>
        ///     入力
        /// </summary>
        /// <param name="inputs"></param>
        public void Input(params IInputType[] inputs)
        {
            foreach (var input in inputs)
                switch (input.GetType().Name)
                {
                    case nameof(AttackInput):
                        _isAttacking = input.IsInput;
                        break;
                    case nameof(DefenseInput):
                        _isDefending = input.IsInput;
                        break;
                }
        }

        /// <summary>
        ///     防御処理
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="deltaTime"></param>
        public async void Defense(OperationType operationType, float deltaTime)
        {
            if (GetDefenceCoolTimer >= _defenceCoolTime)
            {
                //操作方法が手動時、プレイヤーの入力を待つ
                if (operationType == OperationType.Manual && !_isDefending)
                    return;


                OnDefense?.Invoke();
                //軽減率の変更
                _percentageReduction = _percentageReductionValue;
                GetDefenceCoolTimer = 0;
                //魔法が持続開始
                _isDefenceDuration = true;
                //継続時間
                await UniTask.Delay(TimeSpan.FromSeconds(_defenceDuration), DelayType.DeltaTime,
                    PlayerLoopTiming.Update, _token);
                _percentageReduction = _percentageReductionBaseValue;
                //魔法が持続終了
                _isDefenceDuration = false;

                OnEndDefense?.Invoke();
            }
            else if (!_isDefenceDuration) // 防御魔法の持続時間が切れてから、クールダウン回復
            {
                GetDefenceCoolTimer += deltaTime;
            }
        }

        public void StartPose()
        {
            foreach (var particle in _particles)
            {
                if (particle.isPlaying)
                    particle.Pause();
            }
        }

        public void EndPose()
        {
            foreach (var particle in _particles)
            {
                if (particle.isPaused)
                    particle.Play();
            }
        }

        private void GiveDamage()
        {
            _character.TakeDamage(TakeBuff(BuffType.GiveDamage,
                TakeBuff(BuffType.Attack, GetCurrentData.MagicATK)));
        }
    }
}