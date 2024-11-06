using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class SkilUI : MonoBehaviour
    {
        [SerializeField] Slider _slider;
        SkillManager manager;

        private void Awake()
        {
            manager = FindAnyObjectByType<SkillManager>();
            manager.OnCostRecovery += CostSliderChanged;
        }

        public void CostSliderChanged()
        {
            _slider.value = manager.GetCurrentHaveCost / manager.GetMaxCost;
        }
    }
}
