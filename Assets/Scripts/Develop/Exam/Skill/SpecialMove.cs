using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace TeamB.Develop
{
    public class SpecialMove : ISkill
    {
        [SerializeField] float percentageDamage = 30;
        public event Action OnChantingSkill;


        void ISkill.Activation(ICharacter character)
        {
            var rand = UnityEngine.Random.Range(0, 100);
            if (rand <= 30)
            {
                character.TakeDamage(character.GetCurrentData.Hp / 100 * 50); //  30%‚ÌŠm—¦‚ÅŒ»ÝHP‚©‚ç50%‘ŠŽè‚Ì‘Ì—Í‚ðí‚é
            }
            else
            {
                character.TakeDamage(character.GetCurrentData.Hp / 100 * 30); //  70%‚ÌŠm—¦‚ÅŒ»ÝHP‚©‚ç30%‘ŠŽè‚Ì‘Ì—Í‚ðí‚é
            }
        }
            
    }
}
