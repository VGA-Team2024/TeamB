using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using UnityEngine;

namespace TeamB.Develop
{
    public class SpecialMove : ISkill
    {
        [SerializeField] float percentageDamage = 30;
        public event Action OnChantingSkill;
        public void Activation(ICharacter mainCharacter, ICharacter character)
        {
            var rand = UnityEngine.Random.Range(0, 100);
            if (rand <= 30)
            {
                character.TakeDamage(character.GetCurrentData.Hp / 100 * 50); 
            }
            else
            {
                character.TakeDamage(character.GetCurrentData.Hp / 100 * 30); 
            }
        }

            
    }
}
