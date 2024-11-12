using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamB.Develop
{
	/// <summary>
	/// スキルのボタンを管理するクラス
	/// </summary>
	public class SkillButtonUI : MonoBehaviour
	{
		[SerializeField] SkillType _skillType;
		SkillManager _manager;

		private void Awake()
		{
			_manager = FindAnyObjectByType<SkillManager>();
		}

		public void ButtonClick()
		{
			_manager.ActivationSkill(_skillType);
		}
	}
}
