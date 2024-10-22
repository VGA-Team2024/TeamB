using System;
using UnityEngine;

namespace MockUp
{
	/// <summary>
	/// モグラたたきのスコアマネージャー
	/// </summary>
	public class MoleScoreManager : MonoBehaviour
	{
		[SerializeField] private int _upScore;
		private int _getScore;

		public event Action<int> OnScoreChanged;
		public int GetScore => _getScore;

		/// <summary>
		/// スコアアップ
		/// </summary>
		public void ScoreUp()
		{
			_getScore += _upScore;
			OnScoreChanged?.Invoke(_getScore);
		}
	}
}
