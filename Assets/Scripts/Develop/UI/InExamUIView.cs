using TeamB.Data;
using TeamB.Develop;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using TGS2023.BGM;
using TGS2023.SE;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VOICE.SCP;
using Lian = VOICE.Lian.Lian;

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
        [SerializeField] private TMP_Text _skillEffectNameText;
        [SerializeField] private TMP_Text _skillDescriptionText;
        [SerializeField] private TMP_Text _clearText;
        [SerializeField] private Image _attackCoolTimeImage;
        [SerializeField] private Image _DefenceCoolTimeImage;
        [SerializeField] private GameObject skillParticlePlay;
        [SerializeField] private GameObject particlePlay;

        WaveManager _waveManager;
        AllyManager _allyManager;
        Exam _exam;


        protected override void AwakeCall()
        {
            _waveManager = FindAnyObjectByType<WaveManager>();
            _exam = FindAnyObjectByType<Exam>();
            _allyManager = FindAnyObjectByType<AllyManager>();

            CRIAudioManager.BGM.Stop();
            if (GameStatics.ExamState == ExamState.SecondExam)
            {
                CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_007_Battle_Boss));
            }
            else
            {
                CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_003_Battle));
            }


            _exam.OnExamStarted += () =>
            {
                _exam.OnExamUpdated += TimerText;
                _exam.OnExamUpdated += AttackCoolTime;
                _exam.OnExamUpdated += DefenceCoolTime;
            };
            _allyManager.GetAllies.OnSuccessDefence += ScoreChange;
            ScoreChange();
            SkillUI();
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
                    _skillEffectNameText.text = "アステール・プスマ";
                    _skillDescriptionText.text = "確率で相手の現HPの\n半分のダメージを与える";
                    _clearText.text = "敵を倒すと合格！";
                    break;
                case ExamState.FirstExam:
                    _skillNameText.text = "アステール・プスマ";
                    _skillEffectNameText.text = "アステール・プスマ";
                    _skillDescriptionText.text = "確率で相手の現HPの\n半分のダメージを与える";
                    _clearText.text = "45秒生き残ろう!";
                    break;
                case ExamState.SecondExam:
                    _skillNameText.text = "メメント・モリ";
                    _skillEffectNameText.text = "メメント・モリ";
                    _skillDescriptionText.text = "確率で相手を倒す";
                    _clearText.text = "敵を倒すと合格！";
                    break;
            }
        }

        public void SkillParticle()
        {
            switch (GameStatics.ExamState)
            {
                case ExamState.Tutorial:
                    skillParticlePlay.SetActive(true);
                    skillParticlePlay.GetComponent<PlayAction>()!.EventPlay();
                    break;
                case ExamState.FirstExam:
                    skillParticlePlay.SetActive(true);
                    skillParticlePlay.GetComponent<PlayAction>()!.EventPlay();
                    break;
                case ExamState.SecondExam:
                    particlePlay.SetActive(true);
                    particlePlay.GetComponent<PlayAction>()!.EventPlay();
                    break;
            }
        }

        public void ExamStartSE() => CRIAudioManager.SE.Play("SE", nameof(SE.SE_011_Battle_Start));
        public void ExamEndSE() => CRIAudioManager.SE.Play("SE", nameof(SE.SE_012_Battle_End));

        public void ExamStartVOICE()
        {
            if (GameStatics.ExamState == ExamState.SecondExam)
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(Lian.Lian_18));
            }
            else
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(Lian.Lian_17));
            }
        }

        public void ExamEndVOICE()
        {
            if (GameStatics.ExamState == ExamState.SecondExam)
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(Lian.Lian_19));
            }
            else
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(Lian.Lian_20));
            }
        }

    }
}