using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class CharacterUI : MonoBehaviour
    {
        [SerializeField] Slider _slider;
        private AllyManager _allyManager;

        private void Awake()
        {
            _allyManager = FindAnyObjectByType<AllyManager>();
            _allyManager.GetAllies.OnTakeDamage += OnChanged;
        }

        private void OnChanged()
        {
            DebugManager.Log(
                $"{GameStatics.Characters[(int)_allyManager.GetAllies.GetCharacterType].Hp}:{_allyManager.GetAllies.GetCurrentData.Hp}");
            float ratio = _allyManager.GetAllies.GetCurrentData.Hp /
                          GameStatics.Characters[(int)_allyManager.GetAllies.GetCharacterType].Hp;
            _slider.value = ratio;
        }
    }
}
