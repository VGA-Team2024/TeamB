using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MockUp
{
	public class MoleController : MonoBehaviour
	{
		List<Mole> _moles = new List<Mole>();
		MoleScoreManager _scoreManager;
		WackAMoleManager _wackManager;

		private void Start()
		{
			Initialize();
		}

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
