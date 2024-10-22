using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleScoreManager : MonoBehaviour
{
	[SerializeField] private int _upScore;
	private int _score;

	public event Action<int> OnScoreChanged;
	public int Score => _score;

	public void ScoreUp()
	{
		_score += _upScore;
		OnScoreChanged?.Invoke(_score);
	}
}
