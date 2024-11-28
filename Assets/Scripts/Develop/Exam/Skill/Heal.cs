using UnityEngine;
using System;
using TeamB.Develop;
using TeamB.GameSystem.Statics;

namespace TeamB.Develop
{
    /// <summary>
    ///     魔法攻撃力バフ
    /// </summary>
    [Serializable]
    public class Heal : ISkill
    {
        [Tooltip("％表記で"), SerializeField] private float _value;
        [SerializeField] private GameObject _button;
        [SerializeField] BuffUI _buffUI;
        public event Action OnChantingSkill;

        public void Activation(ICharacter mainCharacter, ICharacter character)
        {
            character.TakeDamage(-GameStatics.Characters[(int)GameStatics.NurturingCharacterType].Hp / 100f * _value);
            _button.SetActive(false);
            _buffUI.ReturnBuffUI();
        }
    }
}