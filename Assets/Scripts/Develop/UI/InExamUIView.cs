using TeamB.Data;
using TeamB.Develop;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using TGS2023.BGM;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TeamB.UI
{
	/// <summary>
	/// 試験のUIを管理する
	/// </summary>
	public class InExamUIView : UIView
	{
		[SerializeField] private TMP_Text _scoreText;
		[SerializeField] private TMP_Text _timerText;
		[SerializeField] private TMP_Text _operationText;
		[SerializeField] private TMP_Text _skillNameText;
		[SerializeField] private TMP_Text _skillDescriptionText;
		[SerializeField] private Image _attackCoolTimeImage;
		[SerializeField] private Image _DefenceCoolTimeImage;

		WaveManager _waveManager;
		AllyManager _allyManager;
		Exam _exam;


		protected override void AwakeCall()
		{
			_waveManager = FindAnyObjectByType<WaveManager>();
			_exam = FindAnyObjectByType<Exam>();
			_allyManager = FindAnyObjectByType<AllyManager>();

			// if (CRIAudioManager.BGM.IsPlaying)
			// 	CRIAudioManager.BGM.Stop();
			// CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_003_Battle));

			_exam.OnExamStarted += () =>
			{
				_exam.OnExamUpdated += TimerText;
				_exam.OnExamUpdated += AttackCoolTime;
				_exam.OnExamUpdated += DefenceCoolTime;
			};
			_allyManager.GetAllies.OnSuccessDefence += ScoreChange;
			ScoreChange();
		}

		public void TimerText(float _)
		{
			_timerText.text = $"残り{(_exam.GetMaxTime - _exam.GetCurrentTimer).ToString("F2")}秒";
		}

		public void OperationChange()
		{
			_operationText.text = _exam.GetOperationType == OperationType.Auto ? "Auto" : "Manual";
		}

		public void ScoreChange()
		{
			string text = "";
			switch (GameStatics.ExamState)
			{
				case ExamState.Tutorial:
					text = $"{_allyManager.GetDefenceSuccessCount}回魔法を防いだ";
					break;
				case ExamState.FirstExam:
					text = $"{_allyManager.GetDefenceSuccessCount}回魔法を防いだ";
					break;
				case ExamState.SecondExam:
					text = $"{_allyManager.GetAttackSuccessCount}回魔法を唱えた";
					break;
			}

			if (_scoreText != null)
				_scoreText.text = text;
		}

		public void AttackCoolTime(float _)
		{
			float fill = 1 - _allyManager.GetAllies.GetAttackCoolTimer /
				GameStatics.Characters[(int)GameStatics.NurturingCharacterType].ChantingSpeed;
			_attackCoolTimeImage.fillAmount = fill;
		}

		public void DefenceCoolTime(float _)
		{
			float fill = 1 - _allyManager.GetAllies.GetDefenceCoolTimer /
				_allyManager.GetAllies.GetDefenceCoolTime;
			_DefenceCoolTimeImage.fillAmount = fill;
		}

		public void SkillUI()
		{
			switch (GameStatics.ExamState)
			{
				case ExamState.Tutorial:
					_skillNameText.text = "アステール・プスマ";
					_skillDescriptionText.text = "確率で相手の現HPの半分のダメージを与える";
					break;
				case ExamState.FirstExam:
					_skillNameText.text = "アステール・プスマ";
					_skillDescriptionText.text = "確率で相手の現HPの半分のダメージを与える";
					break;
				case ExamState.SecondExam:
					_skillNameText.text = "メメント・モリ";
					_skillDescriptionText.text = "確率で相手を倒す";
					break;
			}
		}
	}
}
