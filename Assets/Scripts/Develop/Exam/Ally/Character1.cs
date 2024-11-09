using System;
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

		/// <summary> 戦闘用のパラメータ </summary>
		private DataManagement.SpreadSheet.CharacterData _currentData;

		private ActiveType _activeType;
		private ActionType _actionType;
		private float _percentageReduction;
		private float _attackTimer;
		private float _defenceTimer;
		private bool _isInputAction;

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

		public ActionType GetActionType => _actionType;

		public void Initialized()
		{
			_actionType = GameStatics.ExamState == ExamState.FirstExam ? ActionType.Defend : ActionType.Attack;
			_currentData = new(GameStatics.Characters[(int)GameStatics.NurturingCharacterType]);
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
		/// <param name="deltaTime"></param>
		/// <typeparam name="T"></typeparam>
		public void Attack<T>(T characters, OperationType operationType, float deltaTime) where T : ICharacter
		{
			if (_actionType != ActionType.Attack)
				return;

			switch (operationType)
			{
				case OperationType.Auto:
					AutoAttack(characters, deltaTime);
					break;
				case OperationType.Manual:
					ManualAttack(characters, deltaTime);
					break;
				case OperationType.None:
					break;
			}
		}

		private void AutoAttack<T>(T characters, float deltaTime) where T : ICharacter
		{
			if (_attackTimer >= _currentData.ChantingSpeed)
			{
				OnAttack?.Invoke();

				DebugManager.Log(
					$"{_currentData.Card}は{characters.GetCurrentData.Card}に{_currentData.MagicATK}ダメージ与えた");
				characters.TakeDamage(_currentData.MagicATK);
				_attackTimer = 0;

				OnEndAttack?.Invoke();
			}
			else
			{
				_attackTimer += deltaTime;
			}
		}

		private void ManualAttack<T>(T characters, float deltaTime) where T : ICharacter
		{
			
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

		public void Input()
		{
			_isInputAction = true;
		}

		public void Input(params IInputType[] inputs)
		{
			
		}

		public void Defense(OperationType operationType, float deltaTime)
		{
			if (_actionType != ActionType.Defend)
				return;

			if (_defenceTimer >= _defenceCoolTime)
			{
				_percentageReduction = _percentageReductionValue;
				_defenceTimer = 0;
			}
			else
			{
				_defenceTimer += deltaTime;
			}
		}
	}
}
