using System;
using System.Linq;
using UnityEngine;

namespace MockUp
{
	public class WackAMoleManager : MonoBehaviour
	{
		private MiniGameTimeManager _timeManager;
		private MoleUI _moleUI;
		private MoleController _moleController;

		public event Action OnGameStart;
		public event Action OnGameEnd;


		public void GameStart()
		{
			OnGameStart?.Invoke();
			Initialized();
		}

		public void GameEnd()
		{
			OnGameEnd?.Invoke();
		}

		public void Initialized()
		{
			_timeManager = FindAnyObjectByType<MiniGameTimeManager>();
			_timeManager.OnLimit += GameEnd;
			GetComponents<IInitialized>().ToList().ForEach(x => x.Initialize());
		}

		private void OnDisable()
		{
			_timeManager.OnLimit -= GameEnd;
		}
	}

	public interface IInitialized
	{
		public void Initialize();
	}
}
