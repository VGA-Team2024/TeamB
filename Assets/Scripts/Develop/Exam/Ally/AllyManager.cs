using UnityEngine;

namespace TeamB.Develop
{
	/// <summary>
	/// 試験シーンの味方を管理するクラス
	/// </summary>
	public class AllyManager : MonoBehaviour, IExam
	{
		[SerializeReference, SubclassSelector] private ICharacter _allies;

		private EnemyManager _enemyManager;
		private Exam _exam;
		DefenseInput _defenseInput;
		AttackInput _attackInput;

		public ICharacter GetAllies => _allies;

		private void Awake()
		{
			_exam = FindAnyObjectByType<Exam>();
			Initialized();
		}

		private void Initialized()
		{
			_enemyManager = FindAnyObjectByType<EnemyManager>();

			_exam.OnExamStarted += OnStartExam;
			_exam.OnExamEnded += OnEndExam;
		}


		/// <summary>
		/// 味方の攻撃処理
		/// </summary>
		/// <param name="deltaTime"></param>
		private void AlliesAttack(float deltaTime)
		{
			_allies.Attack(_enemyManager.GetCurrentEnemyData, _exam.GetOperationType, deltaTime);
		}

		public void OnStartExam()
		{
			_exam.OnExamUpdated += AlliesAttack;
			_allies.Initialized();
			_allies.OnDeath += () => { _exam.EndExam(); };
		}

		private void OnUpdateExam()
		{
			
		}

		public void OnEndExam()
		{
			_exam.OnExamStarted -= OnStartExam;
			_exam.OnExamEnded -= OnEndExam;
			_exam.OnExamUpdated -= AlliesAttack;
			_allies.Dispose();
		}
	}

	public enum ActionType
	{
		Attack,
		Defend,
		None
	}


	public class AttackInput : IInputType
	{
		private bool _isInput;
		public bool IsInput => _isInput;

		public void ChangeInput(bool value)
		{
			_isInput = value;
		}
	}

	public class DefenseInput : IInputType
	{
		private bool _isInput;
		public bool IsInput => _isInput;

		public void ChangeInput(bool value)
		{
			_isInput = value;
		}
	}
}
