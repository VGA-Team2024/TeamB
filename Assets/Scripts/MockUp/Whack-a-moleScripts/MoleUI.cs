using UnityEngine;
using UnityEngine.UI;

namespace MockUp
{
	/// <summary>
	/// モグラたたきのUI管理
	/// </summary>
	public class MoleUI : MonoBehaviour, IInitialized
	{
		[SerializeField] Text _timerText;
		[SerializeField] Text _scoreText;
		[SerializeField] GameObject _gameOverPanel;
		[SerializeField] Text _gameOverText;
		MiniGameTimeManager _miniGameTimeManager;
		MoleScoreManager _moleScoreManager;
		WackAMoleManager _wackAMoleManager;

		/// <summary>
		/// 制限時間Textの更新
		/// </summary>
		/// <param name="time"></param>
		private void TimerTextUpdate(int time)
		{
			_timerText.text = "Timer:" + (time / 100f);
		}

		/// <summary>
		/// スコアTextの更新
		/// </summary>
		/// <param name="score"></param>
		private void ScoreTextUpdate(int score)
		{
			_scoreText.text = "Score:" + (score);
		}

		/// <summary>
		/// ゲームオーバー時のパネル表示
		/// </summary>
		private void GameOverTextUpdate()
		{
			_gameOverPanel.SetActive(true);
			_timerText.gameObject.SetActive(false);
			_scoreText.gameObject.SetActive(false);
			_gameOverText.text = $"Score:{_moleScoreManager.GetScore}";
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
			_moleScoreManager = FindAnyObjectByType<MoleScoreManager>();
			_miniGameTimeManager = FindAnyObjectByType<MiniGameTimeManager>();
			_wackAMoleManager = FindAnyObjectByType<WackAMoleManager>();


			if (_miniGameTimeManager)
				_miniGameTimeManager.OnUpdate += TimerTextUpdate;
			if (_moleScoreManager)
				_moleScoreManager.OnScoreChanged += ScoreTextUpdate;
			_wackAMoleManager.OnGameEnd += GameOverTextUpdate;
		}

		private void OnDisable()
		{
			if (_miniGameTimeManager)
				_miniGameTimeManager.OnUpdate -= TimerTextUpdate;
			if (_moleScoreManager)
				_moleScoreManager.OnScoreChanged -= ScoreTextUpdate;
			_wackAMoleManager.OnGameEnd -= GameOverTextUpdate;
		}
	}
}
