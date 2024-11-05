using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class StudentCouncilPresidentUI : MonoBehaviour
    {
        [SerializeField] Slider _slider;
        private EnemyManager _enemyManager;

        private void Awake()
        {
            _enemyManager = FindAnyObjectByType<EnemyManager>();
            _enemyManager.GetCurrentEnemyData.OnTakeDamage += OnChanged;
        }

        private void OnChanged()
        {
            float ratio = _enemyManager.GetCurrentEnemyData.GetCurrentData.Hp /
                          GameStatics.Characters[(int)_enemyManager.GetCurrentEnemyData.GetCharacterType].Hp;
            _slider.value = ratio;
        }
    }
}