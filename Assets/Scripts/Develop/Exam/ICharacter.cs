using System.Collections.Generic;
using System;
using DataManagement;
using DataManagement.SpreadSheet;

namespace TeamB.Develop
{
	/// <summary>
	/// 戦闘キャラ用のインターフェース
	/// </summary>
	public interface ICharacter
	{
		public CharacterType GetCharacterType { get; }
		public DataManagement.SpreadSheet.CharacterData GetCurrentData { get; }


		#region Actions

		public event Action OnDeath;
		public event Action OnAttack;
		public event Action OnEndAttack;
		public event Action OnTakeDamage;

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
		/// 試験終了後に行う処理
		/// </summary>
		public void Dispose();
	}

	/// <summary>
	/// 味方クラスが継承するべきインターフェース
	/// </summary>
	public interface IAlly : ICharacter
	{
		public ActionType GetActionType { get; }

		public void Input(params IInputType[] inputs);

		/// <summary>
		/// 
		/// </summary>
		public void Defense(OperationType operationType, float deltaTime);
	}

	/// <summary>
	/// 敵クラスが継承するべきインターフェース
	/// </summary>
	public interface IEnemy : ICharacter
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
