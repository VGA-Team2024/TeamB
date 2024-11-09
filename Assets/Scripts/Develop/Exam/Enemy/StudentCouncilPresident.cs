using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DataManagement;
using DataManagement.SpreadSheet;
using TeamB.Data;
using TeamB.GameSystem.Statics;

namespace TeamB.Develop
{
	/// <summary>
	/// 生徒会長の行動を管理するクラス
	/// </summary>
	public class StudentCouncilPresident : IEnemy
	{
		[SerializeField] CharacterType _characterType;

		private DataManagement.SpreadSheet.CharacterData _currentData;
		private float _attackTimer;
		private int _currentForm = 1;

		public event Action OnDeath;
		public event Action OnAttack;
		public event Action OnEndAttack;
		public event Action OnTakeDamage;
		public event Action OnParamUpdate;
		public event Action OnNextForm;

		public DataManagement.SpreadSheet.CharacterData GetCurrentData => _currentData;
		public CharacterType GetCharacterType => _characterType;
		public int GetCurrentForm => _currentForm;

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
		public void Attack<T>(T characters, OperationType _, float deltaTime) where T : ICharacter
		{
			if (_attackTimer >= _currentData.ChantingSpeed)
			{
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
	}
}
