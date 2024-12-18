using TeamB.Data;
using TeamB.Develop;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
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
        [SerializeField] private TMP_Text _waveText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _operationText;
        [SerializeField] private Image _attackCoolTimeImage;
        [SerializeField] private Image _DefenceCoolTimeImage;

        WaveManager _waveManager;
        AllyManager _allyManager;
        Exam _exam;


        /// <summary>
        /// 試験を終えるタイミングで呼び出す
        /// </summary>
        public void ExitExam()
        {
            // 乱数をはじく
            var tmp = Random.Range(1, 10);
            if (tmp > 5)
            {
                SceneLoader.LoadScene("moch_CharmUp");
                GameStatics.PrevGameState = GameState.Exam;
            }
            else
            {
                SceneLoader.LoadScene("moch_SuddenlyEvent");
                GameStatics.PrevGameState = GameState.Exam;
            }
        }

        protected override void AwakeCall()
        {
            _waveManager = FindAnyObjectByType<WaveManager>();
            _exam = FindAnyObjectByType<Exam>();
            _allyManager = FindAnyObjectByType<AllyManager>();
            
            //CRIAudioManager.BGM.Stop();

            _waveManager.OnNextWave += WaveText;
            _exam.OnExamUpdated += TimerText;
            _exam.OnExamUpdated += AttackCoolTime;
            _exam.OnExamUpdated += DefenceCoolTime;
            _allyManager.GetAllies.OnSuccessDefence += ScoreChange;
            ScoreChange();
        }

        public void WaveText()
        {
            _waveText.text = $"残り{GameConsts.MaxWave - _waveManager.GetCurrentWave + 1}ウェーブ";
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
    }
}
