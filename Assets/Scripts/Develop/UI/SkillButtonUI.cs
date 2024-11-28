using System;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    /// <summary>
    /// スキルのボタンを管理するクラス
    /// </summary>
    public class SkillButtonUI : MonoBehaviour
    {
        [SerializeField] SkillType _skillType;
        [SerializeField] Image _skillImage;
        SkillManager _manager;
        private float cost;

        private void Awake()
        {
            _manager = FindAnyObjectByType<SkillManager>();
            cost = _manager.SearchSkill(_skillType).cost;
        }

        private void Update()
        {
            if(!_skillImage)
                return;
            if (cost <= _manager.GetCurrentHaveCost)
            {
                _skillImage.fillAmount = 0f;
            }
            else
            {
                _skillImage.fillAmount = 1f;
            }
        }

        public void ButtonClick()
        {
            _manager.ActivationSkill(_skillType);
            
        }
    }
}