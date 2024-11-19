using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataManagement;
using DataManagement.SpreadSheet;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;

namespace TeamB.Develop
{
    /// <summary>
    /// 生徒会長の行動を管理するクラス
    /// </summary>
    public class StudentCouncilPresident : IEnemy
    {
        #region serializeFields

        [SerializeField] private CharacterType _characterType;
        [SerializeField] private ParticleSystem _attackParticle;
        [SerializeField] private ParticleCallBack _attackParticleCallBack;
        [SerializeField] private float _waitAttack;

        #endregion

        #region privates

        private DataManagement.SpreadSheet.CharacterData _currentData;
        private List<IBuff> _haveBuffs = new();
        private List<IBuff> _haveDeBuffs = new();
        private ICharacter _targetCharacter;
        private float _attackTimer;
        private int _currentForm = 1;

        #endregion

        #region Actions

        public event Action OnDeath;
        public event Action OnAttack;
        public event Action OnEndAttack;
        public event Action OnTakeDamage;
        public event Action OnAddBuff;
        public event Action OnRemoveBuff;
        public event Action OnAddDeBuff;
        public event Action OnRemoveDeBuff;
        public event Action OnParamUpdate;
        public event Action OnNextForm;

        #endregion

        #region properties

        public DataManagement.SpreadSheet.CharacterData GetCurrentData => _currentData;
        public List<IBuff> GetHaveBuffs => _haveBuffs;
        public List<IBuff> GetHaveDeBuffs => _haveDeBuffs;
        public CharacterType GetCharacterType => _characterType;
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
            if (_attackTimer >= TakeBuff(BuffType.CastingSpeed, _currentData.ChantingSpeed))
            {
                _targetCharacter = characters;
                _attackParticleCallBack.OnCallBack -= GiveDamage;
                float rand = UnityEngine.Random.Range(0, 100);
                if (rand <= TakeBuff(BuffType.HitRate, _currentData.HitRate))
                {
                    OnAttack?.Invoke();
                 
                    _attackTimer = 0;
                    _attackParticle.Play();

                    _attackParticleCallBack.OnCallBack += GiveDamage;

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

            DebugManager.Log(
                $"生徒会長は{damage}ダメージ受けた");
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
                OnDeath?.Invoke();
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
            _attackParticle.Pause();
        }

        public void EndPose()
        {
            _attackParticle.Play();
        }
    }
}