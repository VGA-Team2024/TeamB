using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MockUp
{
	/// <summary>
	/// モグラすべてを管理する機能
	/// </summary>
	public class MoleController : MonoBehaviour
	{
		List<Mole> _moles = new List<Mole>();
		MoleScoreManager _scoreManager;
		WackAMoleManager _wackManager;

		private void Start()
		{
			Initialize();
		}

		/// <summary>
		/// モグラ全ての初期化
		/// </summary>
		private void MolesInitialize()
		{
			_moles = GetComponentsInChildren<Mole>().ToList();
			foreach (var mole in _moles)
			{
				mole.SpawnMole();
				if (_scoreManager)
					mole.OnClicked += _scoreManager.ScoreUp;
				if (_wackManager)
					_wackManager.OnGameEnd += mole.EndMole;
			}
		}
		
		/// <summary>
		/// Moleコントローラーの初期化
		/// </summary>
		public void Initialize()
		{
			_scoreManager = FindObjectOfType<MoleScoreManager>();
			_wackManager = FindAnyObjectByType<WackAMoleManager>();
			if (_wackManager)
			{
				_wackManager.OnGameStart += MolesInitialize;
			}
		}

		private void OnDisable()
		{
			foreach (var mole in _moles)
			{
				if (_scoreManager)
					mole.OnClicked -= _scoreManager.ScoreUp;
				if (_wackManager)
				{
					_wackManager.OnGameEnd -= mole.EndMole;
				}
			}

			_wackManager.OnGameStart -= MolesInitialize;
		}
	}
}
