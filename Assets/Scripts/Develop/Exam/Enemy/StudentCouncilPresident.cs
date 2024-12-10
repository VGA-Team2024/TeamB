using System;
using System.Collections.Generic;
using System.Linq;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// 生徒会長の行動を管理するクラス
    /// </summary>
    public class StudentCouncilPresident : IEnemy
    {
        #region serializeFields

        [SerializeField] private CharacterType _characterType;
        [SerializeField] private GameObject _attackParticle;
        [SerializeField] private Transform _attackParticleTrans;
        [SerializeField] private float _waitAttack;
        [SerializeField] private float _downTime;

        #endregion

        #region privates

        private DataManagement.SpreadSheet.CharacterData _currentData;
        private List<IBuff> _haveBuffs = new();
        private List<IBuff> _haveDeBuffs = new();
        private List<ParticleSystem> _particles = new();
        private ICharacter _targetCharacter;
        private float _attackTimer;
        private float _downTimer;
        private int _currentForm = 1;
        [SerializeField] private AbnormalCondition _currentCondition;

        #endregion

        #region Actions

        public event Action OnDeath;
        public event Action OnAttack;
        public event Action OnEndAttack;
        public event Action OnTakeDamage;
        public event Action OnTakeHeal;
        public event Action OnAddBuff;
        public event Action OnRemoveBuff;
        public event Action OnAddDeBuff;
        public event Action OnRemoveDeBuff;
        public event Action OnParamUpdate;
        public event Action OnNextForm;
        public event Action OnDown;

        #endregion

        #region properties

        public DataManagement.SpreadSheet.CharacterData GetCurrentData => _currentData;
        public List<IBuff> GetHaveBuffs => _haveBuffs;
        public List<IBuff> GetHaveDeBuffs => _haveDeBuffs;
        public CharacterType GetCharacterType => _characterType;
        public AbnormalCondition GetCurrentCondition => _currentCondition;
        public int GetCurrentForm => _currentForm;

        #endregion

        public void Initialized()
        {
            _currentData = new(GameStatics.Characters[(int)_characterType]);
        }

        /// <summary>
        /// 外からキャラのデータを変更する関数
        /// </summary>
        public void RegistrationType(CharacterType type)
        {
            _characterType = type;
            _currentData = new(GameStatics.Characters[(int)_characterType]);
        }

        /// <summary>
        /// 攻撃処理
        /// </summary>
        public async void Attack<T>(T characters, OperationType _, float deltaTime) where T : ICharacter
        {
            if (_currentCondition == AbnormalCondition.Stunned)
            {
                if (_downTimer >= _downTime)
                {
                    _downTimer = 0;
                    _currentData.Hp = GameStatics.Characters[(int)_characterType].Hp;
                    _currentCondition = AbnormalCondition.Normal;
                }
                else
                {
                    _downTimer += deltaTime;
                }
                return; 
                //スタン中の場合
            }

            if (_attackTimer >= TakeBuff(BuffType.CastingSpeed, _currentData.ChantingSpeed))
            {
                _targetCharacter = characters;
                GameObject attackParticle = GameObject.Instantiate(_attackParticle, _attackParticleTrans.position, _attackParticle.transform.rotation);
                ParticleSystem attackParticleSystem = attackParticle.GetComponent<ParticleSystem>();
                attackParticleSystem.Play();
                _particles.Add(attackParticleSystem);
                
                float rand = UnityEngine.Random.Range(0, 100);
                if (rand <= TakeBuff(BuffType.HitRate, _currentData.HitRate))
                {
                    OnAttack?.Invoke();

                    _attackTimer = 0;
                    
                    ParticleCallBack particleCallBack = attackParticle.GetComponent<ParticleCallBack>();
                    particleCallBack.OnCallBack += GiveDamage;
                    particleCallBack.OnCallBack += () =>
                    {
                        _particles.Remove(attackParticleSystem);
                        GameObject.Destroy(attackParticle);
                    };

                    OnEndAttack?.Invoke();
                }
                else
                {
                    _attackTimer = 0;
                    DebugManager.Log("生徒会長の攻撃が外れた");
                }
            }
            else
            {
                _attackTimer += deltaTime;
            }
        }

        private void GiveDamage()
        {
            _targetCharacter.TakeDamage(TakeBuff(BuffType.De_GiveDamage, TakeBuff(BuffType.GiveDamage,
                TakeBuff(BuffType.Attack, _currentData.MagicATK))));
        }

        /// <summary>
        /// 受ダメージ処理
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            if (_currentData.Hp <= 0)
                return;

            //HPの更新
            _currentData.Hp -= damage;
            OnTakeDamage?.Invoke();

            //形態変化
            if (_currentData.Hp <= GameStatics.Characters[(int)_characterType].Hp / GameConsts.MaxWave *
                (GameConsts.MaxWave - GetCurrentForm) && _currentData.Hp > 0)
            {
                _currentForm++;
                OnNextForm?.Invoke();
            }

            //死亡時処理
            if (_currentData.Hp <= 0)
            {
                switch (GameStatics.ExamState)
                {
                    case ExamState.Tutorial:
                        OnDeath?.Invoke();
                        break;
                    case ExamState.FirstExam:
                        _currentCondition = AbnormalCondition.Stunned;
                        OnDown?.Invoke();
                        break;
                    case ExamState.SecondExam:
                        OnDeath?.Invoke();
                        break;
                }
            }
        }


        /// <summary>
        /// バフ追加
        /// </summary>
        /// <param name="buff"></param>
        public void AddBuff(IBuff buff)
        {
            _haveBuffs.Add(buff);
        }

        /// <summary>
        /// バフ解除
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RemoveBuff(float deltaTime)
        {
            for (int i = 0; i < _haveBuffs.Count; i++)
            {
                if (_haveBuffs[i].Timer(deltaTime))
                {
                    _haveBuffs.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// デバフ追加
        /// </summary>
        /// <param name="buff"></param>
        public void AddDeBuff(IBuff buff)
        {
            _haveDeBuffs.Add(buff);
        }

        /// <summary>
        /// デバフ解除
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RemoveDeBuff(float deltaTime)
        {
            for (int i = 0; i < _haveDeBuffs.Count; i++)
            {
                if (_haveDeBuffs[i].Timer(deltaTime))
                {
                    _haveDeBuffs.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// バフ、デバフの適用
        /// </summary>
        /// <param name="buffType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public float TakeBuff(BuffType buffType, float value)
        {
            //加算バフ、デバフ
            foreach (var buff in _haveBuffs.Where(x => x.GetBuffType == buffType))
            {
                if (buff.GetCalculationMethod == CalculationMethod.Addition &&
                    buff.GetBuffType == buffType)
                    value += buff.GetValue;
            }

            foreach (var Debuff in _haveDeBuffs.Where(x => x.GetBuffType == buffType))
            {
                if (Debuff.GetCalculationMethod == CalculationMethod.Addition &&
                    Debuff.GetBuffType == buffType)
                    value += Debuff.GetValue;
            }

            //乗算バフ、デバフ
            foreach (var buff in _haveBuffs.Where(x => x.GetBuffType == buffType))
            {
                if (buff.GetCalculationMethod == CalculationMethod.Multiplication &&
                    buff.GetBuffType == buffType)
                    value *= buff.GetValue;
            }

            foreach (var Debuff in _haveDeBuffs.Where(x => x.GetBuffType == buffType))
            {
                if (Debuff.GetCalculationMethod == CalculationMethod.Multiplication &&
                    Debuff.GetBuffType == buffType)
                    value *= Debuff.GetValue;
            }

            return value;
        }

        public void AttackCancel()
        {
        }

        public void Dispose()
        {
            OnDeath = default;
            OnParamUpdate = default;
            OnAttack = default;
            OnTakeDamage = default;
            OnEndAttack = default;
            OnNextForm = default;
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
    }
}