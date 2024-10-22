using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MockUp
{
	/// <summary>
	/// モグラたたきのタイムマネージャー
	/// </summary>
	public class MiniGameTimeManager : MonoBehaviour, IInitialized
	{
		[SerializeField,Range(20f, 30f)] private float _timeLimit;

		public event Action OnStart;
		public event Action<int> OnUpdate;
		public event Action OnLimit;
		

		/// <summary>
		/// タイマー機能
		/// </summary>
		public async void TimerUpdate()
		{
			OnStart?.Invoke();
			for (int i = 0; i < _timeLimit * 100; i++)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
				OnUpdate?.Invoke(i);
			}
			OnLimit?.Invoke();
		}

		public void Initialize()
		{
			TimerUpdate();
		}
	}
}
