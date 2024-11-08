using System;
using TeamB.GameSystem.Statics;

namespace TeamB.Develop
{
    /// <summary>
    /// キャラクターを管理するクラス : サンプルクラス
    /// このように実装すれば量産できるよ
    /// </summary>
    public class Character1 : ICharacter
    {
        /// <summary> 戦闘用のパラメータ </summary>
        private DataManagement.SpreadSheet.CharacterData _currentData;
        private float _attackTimer;

        #region Actions
        
        public event Action OnDeath;
        public event Action OnAttack;
        public event Action OnEndAttack;
        public event Action OnTakeDamage;
        public event Action OnParamUpDate;

        #endregion
        
        // 戦闘用のパラメータ
        public DataManagement.SpreadSheet.CharacterData GetCurrentData => _currentData;
        // キャラの種類
        public CharacterType GetCharacterType => GameStatics.NurturingCharacterType;

        public void Initialized()
        {
            _currentData =  new (GameStatics.Characters[(int)GameStatics.NurturingCharacterType]);
            OnParamUpDate?.Invoke();
        }

        /// <summary>
        /// キャラの登録処理
        /// </summary>
        /// <param name="type"></param>
        public void RegistrationType(CharacterType type)
        {
            GameStatics.NurturingCharacterType = type;
        }

        /// <summary>
        /// 攻撃処理
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="deltatime"></param>
        /// <typeparam name="T"></typeparam>
        public void Attack<T>(T characters, float deltatime) where T : ICharacter
        {
            if (_attackTimer >= _currentData.ChantingSpeed)
            {
                OnAttack?.Invoke();
                
                DebugManager.Log($"{_currentData.Card}は{characters.GetCurrentData.Card}に{_currentData.MagicATK}ダメージ与えた");
                characters.TakeDamage(_currentData.MagicATK);
                _attackTimer = 0;

                OnEndAttack?.Invoke();
            }
            else
            {
                _attackTimer += deltatime;
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
            _currentData.Hp -= damage;
            OnTakeDamage?.Invoke();

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
    }
}