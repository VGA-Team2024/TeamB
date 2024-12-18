using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using UnityEngine;

namespace TeamB.Develop
{
    public class InstantKill : ISkill
    {
        [SerializeField] float percentageDamage = 50;
        public event Action OnChantingSkill;
        public void Activation(ICharacter mainCharacter, ICharacter character)
        {
            var rand = UnityEngine.Random.Range(0, 100);
            if (rand <= 10)
            {
                character.TakeDamage(character.GetCurrentData.Hp);
            }
            else
            {
                character.TakeDamage(character.GetCurrentData.Hp / 100 * 50);
            }
        }

        
    }
}
