using System;
using System.Collections.Generic;
using TeamB.GameSystem;

namespace TeamB.Develop
{
    /// <summary>
    /// 戦闘キャラ用のインターフェース
    /// </summary>
    public interface ICharacter
    {
        #region Properties

        public CharacterType GetCharacterType { get; }
        public DataManagement.SpreadSheet.CharacterData GetCurrentData { get; }
        public List<IBuff> GetHaveBuffs { get; }
        public List<IBuff> GetHaveDeBuffs { get; }

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

        #endregion

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialized();

        /// <summary>
        /// キャラの登録処理
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        public void RegistrationType(CharacterType type);

        /// <summary>
        /// 攻撃処理
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="deltaTime"></param>
        /// <typeparam name="T"></typeparam>
        public void Attack<T>(T characters, OperationType operationType, float deltaTime) where T : ICharacter;

        public void AttackCancel();

        /// <summary>
        /// ダメージ受ける処理
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage);

        /// <summary>
        /// バフ追加
        /// </summary>
        /// <param name="buff"></param>
        public void AddBuff(IBuff buff);

        /// <summary>
        /// バフ解除
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RemoveBuff(float deltaTime);

        /// <summary>
        /// デバフ追加
        /// </summary>
        /// <param name="buff"></param>
        public void AddDeBuff(IBuff buff);

        /// <summary>
        /// デバフ解除
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RemoveDeBuff(float deltaTime);

        public float TakeBuff(BuffType buffType, float value);

        /// <summary>
        /// 試験終了後に行う処理
        /// </summary>
        public void Dispose();
    }

    /// <summary>
    /// 味方クラスが継承するべきインターフェース
    /// </summary>
    public interface IAlly : ICharacter, IPoseObject
    {
        public ActionType GetActionType { get; }

        public event Action OnTakeHeal;
        public event Action OnDefense;
        public event Action OnEndDefense;
        public event Action OnSuccessDefence;
        public float GetAttackCoolTimer { get; }
        public float GetDefenceCoolTimer { get; }
        public float GetDefenceCoolTime { get; }

        public void Input(params IInputType[] inputs);

        /// <summary>
        /// 
        /// </summary>
        public void Defense(OperationType operationType, float deltaTime);
    }

    /// <summary>
    /// 敵クラスが継承するべきインターフェース
    /// </summary>
    public interface IEnemy : ICharacter, IPoseObject
    {
        public event Action OnNextForm;
        public int GetCurrentForm { get; }
    }

    /// <summary>
    /// 入力クラスを作成する時に継承する（paramを使うためのインターフェース）
    /// </summary>
    public interface IInputType
    {
        public bool IsInput { get; }
    }
}