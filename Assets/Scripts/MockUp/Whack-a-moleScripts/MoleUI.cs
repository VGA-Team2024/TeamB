using UnityEngine;
using UnityEngine.UI;

namespace MockUp
{
	public class MoleUI : MonoBehaviour, IInitialized
	{
		[SerializeField] Text _timerText;
		[SerializeField] Text _scoreText;
		[SerializeField] GameObject _gameOverPanel;
		[SerializeField] Text _gameOverText;
		MiniGameTimeManager _miniGameTimeManager;
		MoleScoreManager _moleScoreManager;
		WackAMoleManager _wackAMoleManager;

		private void TimerTextUpdate(int time)
		{
			_timerText.text = "Timer:" + (time / 100f);
		}

		private void ScoreTextUpdate(int score)
		{
			_scoreText.text = "Score:" + (score);
		}

		private void GameOverTextUpdate()
		{
			_gameOverPanel.SetActive(true);
			_timerText.gameObject.SetActive(false);
			_scoreText.gameObject.SetActive(false);
			_gameOverText.text = $"Score:{_moleScoreManager.Score}";
		}


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
