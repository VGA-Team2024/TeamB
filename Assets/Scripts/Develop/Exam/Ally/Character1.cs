using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TeamB.Data;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// キャラクターを管理するクラス : サンプルクラス
    /// このように実装すれば量産できるよ
    /// </summary>
    public class Character1 : IAlly
    {
        #region SerializedFields

        [SerializeField] private float _percentageReductionValue = 0.75f;
        [SerializeField] private float _defenceCoolTime = 1f;
        [SerializeField] private float _defenceDurationTime = 1f;

        #endregion

        #region Privates

        /// <summary> 戦闘用のパラメータ </summary>
        private DataManagement.SpreadSheet.CharacterData _currentData;

        private List<IBuff> _haveBuffs = new();
        private List<IBuff> _haveDeBuffs = new();

        private CancellationToken _token;

        private ActionType _actionType;

        private float _percentageReduction;
        private float _attackTimer;
        private float _defenceTimer;
        private float _defenceDurationTimer;
        private float _percentageReductionBaseValue = 1;

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
        public event Action OnTakeDamage;
        public event Action OnAddBuff;
        public event Action OnRemoveBuff;
        public event Action OnAddDeBuff;
        public event Action OnRemoveDeBuff;
        public event Action OnParamUpDate;

        #endregion

        #region Properties

        // 戦闘用のパラメータ
        public DataManagement.SpreadSheet.CharacterData GetCurrentData => _currentData;
        public List<IBuff> GetHaveBuffs => _haveBuffs;
        public List<IBuff> GetHaveDeBuffs => _haveDeBuffs;

        // キャラの種類
        public CharacterType GetCharacterType => GameStatics.NurturingCharacterType;

        public ActionType GetActionType => _actionType;

        #endregion

        public void Initialized()
        {
            DebugManager.Log($"主人公の攻撃力{GameStatics.Characters[(int)GameStatics.NurturingCharacterType].MagicATK}");
            _actionType = GameStatics.ExamState == ExamState.FirstExam ? ActionType.Defend : ActionType.Attack;
            _currentData = new(GameStatics.Characters[(int)GameStatics.NurturingCharacterType]);
            _token = new CancellationTokenSource().Token;
            _percentageReduction  = _percentageReductionBaseValue;
            OnParamUpDate?.Invoke();
        }

        /// <summary>
        /// キャラの登録処理
        /// </summary>
        /// <param name="type"></param>
        public void RegistrationType(CharacterType type)
        {
            GameStatics.NurturingCharacterType = type;
            _currentData = new(GameStatics.Characters[(int)GameStatics.NurturingCharacterType]);
        }

        /// <summary>
        /// 攻撃処理
        /// </summary>
        public void Attack<T>(T characters, OperationType operationType, float deltaTime) where T : ICharacter
        {
            if (_actionType != ActionType.Attack)
                return;

            if (_attackTimer >= TakeBuff(BuffType.CastingSpeed, _currentData.ChantingSpeed))
            {
                //操作方法が自動時、ゲームプレイヤーの入力を待つ
                if (operationType == OperationType.Manual && !_isAttacking)
                    return;

                float rand = UnityEngine.Random.Range(0, 100);
                if (rand <= TakeBuff(BuffType.HitRate, _currentData.HitRate))
                {
                    OnAttack?.Invoke();

                    DebugManager.Log($"主人公の攻撃力{_currentData.MagicATK}");
                    characters.TakeDamage(TakeBuff(BuffType.GiveDamage,
                        TakeBuff(BuffType.Attack, _currentData.MagicATK)));
                    _attackTimer = 0;

                    OnEndAttack?.Invoke();
                }
                else
                {
                    _attackTimer = 0;
                    DebugManager.Log("主人公の攻撃が外れた");
                }
            }
            else
            {
                _attackTimer += deltaTime;
            }
        }

        public void AttackCancel()
        {
        }

        /// <summary>
        /// 攻撃を受ける際の処理
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            if (_currentData.Hp <= 0)
                return;

            //HPの更新
            _currentData.Hp -= damage * _percentageReduction;
            OnTakeDamage?.Invoke();

            DebugManager.Log(
                $"主人公は{damage * _percentageReduction}ダメージ受けた");
            //死亡時処理
            if (_currentData.Hp <= 0)
            {
                DebugManager.Log($"{_currentData.Card}は敗北した");
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
            DebugManager.Log($"{nameof(buff)}のバフ追加{buff.GetValue}");
            OnAddBuff?.Invoke();
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
                    OnRemoveBuff?.Invoke();
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
            OnAddDeBuff?.Invoke();
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

                    OnRemoveDeBuff?.Invoke();
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
            List<IBuff> buffs = _haveBuffs.Where(x => x.GetBuffType == buffType).ToList();
            if (buffs.Count == 0)
                return value;

            float buffed = value;
            
            foreach (var buff in buffs)
            {
                //加算バフ、デバフ
                if (buff.GetCalculationMethod == CalculationMethod.Addition &&
                    buff.GetBuffType == buffType)
                    buffed += buff.GetValue;
                DebugManager.Log($"{buffType.ToString()}バフ");
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
        }

        /// <summary>
        /// 入力
        /// </summary>
        /// <param name="inputs"></param>
        public void Input(params IInputType[] inputs)
        {
            foreach (var input in inputs)
            {
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
        }

        /// <summary>
        /// 防御処理
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="deltaTime"></param>
        public async void Defense(OperationType operationType, float deltaTime)
        {
            if (_actionType != ActionType.Defend)
                return;

            if (_defenceTimer >= _defenceCoolTime)
            {
                //操作方法が手動時、プレイヤーの入力を待つ
                if (operationType == OperationType.Manual && !_isDefending)
                    return;

                OnDefense?.Invoke();
                DebugManager.Log("防御魔法を展開した");
                //軽減率の変更
                _percentageReduction = _percentageReductionValue;
                _defenceTimer = 0;
                //魔法が持続開始
                _isDefenceDuration = true;
                //継続時間
                await UniTask.Delay(TimeSpan.FromSeconds(_defenceDurationTime), DelayType.DeltaTime,
                    PlayerLoopTiming.Update, _token);
                _percentageReduction = _percentageReductionBaseValue;
                //魔法が持続終了
                _isDefenceDuration = false;
                DebugManager.Log("防御魔法が消えた");

                OnEndDefense?.Invoke();
            }
            else if (!_isDefenceDuration) // 防御魔法の持続時間が切れてから、クールダウン回復
            {
                _defenceTimer += deltaTime;
            }
        }
    }
}