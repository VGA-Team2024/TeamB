using TeamB.Data;
using TeamB.Develop;
using TeamB.GameSystem;
using TeamB.GameSystem.Statics;
using TeamB.InGameData.Data;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.UI
{
    /// <summary>
    /// 試験のUIを管理する
    /// </summary>
    public class InExamUIView : UIView
    {
        [SerializeField] private Text _soulText;
        [SerializeField] private Text _waveText;
        [SerializeField] private Text _TimerText;
        
        SoulManager _soulManager;
        WaveManager _waveManager;
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
            _soulManager = FindAnyObjectByType<SoulManager>();
            _waveManager = FindAnyObjectByType<WaveManager>();
            _exam = FindAnyObjectByType<Exam>();

            _soulManager.OnAddSoul += SoulText;
            _waveManager.OnNextWave += WaveText;
            _exam.OnExamUpdated += TimerText;
        }

        public void SoulText()
        {
            DebugManager.Log(_soulManager.GetCurrentSoul);
            _soulText.text = $"回収した魂{_soulManager.GetCurrentSoul.ToString("00")}個";
        }
        public void WaveText()
        {
            DebugManager.Log(_waveManager.GetCurrentWave);
            _waveText.text = $"残り{GameConsts.MaxWave - _waveManager.GetCurrentWave + 1}ウェーブ";
        }
        public void TimerText(float _)
        {
            _TimerText.text = $"残り{(_exam.GetMaxTime - _exam.GetCurrentTimer).ToString("F2")}秒";
        }
    }
}