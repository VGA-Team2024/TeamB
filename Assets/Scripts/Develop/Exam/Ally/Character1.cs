using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TeamB.Data;
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
        [SerializeField] private float _percentageReductionValue = 0.75f;
        [SerializeField] private float _defenceCoolTime = 1f;
        [SerializeField] private float _defenceDurationTime = 1f;

        /// <summary> 戦闘用のパラメータ </summary>
        private DataManagement.SpreadSheet.CharacterData _currentData;

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

        #region Actions

        public event Action OnDeath;
        public event Action OnAttack;
        public event Action OnEndAttack;
        public event Action OnDefence;
        public event Action OnEndDefence;
        public event Action OnTakeDamage;
        public event Action OnParamUpDate;

        #endregion

        // 戦闘用のパラメータ
        public DataManagement.SpreadSheet.CharacterData GetCurrentData => _currentData;

        // キャラの種類
        public CharacterType GetCharacterType => GameStatics.NurturingCharacterType;

        public ActionType GetActionType => _actionType;

        public void Initialized()
        {
            _actionType = GameStatics.ExamState == ExamState.FirstExam ? ActionType.Defend : ActionType.Attack;
            _currentData = new(GameStatics.Characters[(int)GameStatics.NurturingCharacterType]);
            _token = new CancellationTokenSource().Token;
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


            if (_attackTimer >= _currentData.ChantingSpeed)
            {
                //操作方法が自動時、ゲームプレイヤーの入力を待つ
                if (operationType == OperationType.Manual && !_isAttacking)
                    return;

                OnAttack?.Invoke();

                characters.TakeDamage(_currentData.MagicATK);
                _attackTimer = 0;

                OnEndAttack?.Invoke();
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


        public void Dispose()
        {
            OnAttack = default;
            OnDeath = default;
            OnEndAttack = default;
            OnTakeDamage = default;
            OnParamUpDate = default;
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

        public async void Defense(OperationType operationType, float deltaTime)
        {
            if (_actionType != ActionType.Defend)
                return;

            if (_defenceTimer >= _defenceCoolTime)
            {
                //操作方法が手動時、プレイヤーの入力を待つ
                if (operationType == OperationType.Manual && !_isDefending)
                    return;

                OnDefence?.Invoke();
                DebugManager.Log("防御魔法を展開した");
                _percentageReduction = _percentageReductionValue;
                _defenceTimer = 0;
                _isDefenceDuration = true;
                await UniTask.Delay(TimeSpan.FromSeconds(_defenceDurationTime), DelayType.DeltaTime,
                    PlayerLoopTiming.Update, _token);
                _percentageReduction = _percentageReductionBaseValue;
                _isDefenceDuration = false;
                DebugManager.Log("防御魔法が消えた");

                OnEndDefence?.Invoke();
            }
            else if (!_isDefenceDuration) // 防御魔法の持続時間が切れてから、クールダウン回復
            {
                _defenceTimer += deltaTime;
            }
        }
    }
}