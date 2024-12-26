using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using TGS2023.SE;
using UnityEngine;

namespace TeamB.Develop
{
    public class InstantKill : ISkill
    {
        public event Action OnChantingSkill;

        public void Activation(ICharacter mainCharacter, ICharacter character)
        {
            CRIAudioManager.SE.Play("SE", nameof(SE.SE_016_Skill_Execute));
            if (GameStatics.GetRandomNumber(2) == 0)
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(VOICE.Lian.Lian.Lian_22));
            }
            else
            {
                CRIAudioManager.VOICE.Play("Lian", nameof(VOICE.Lian.Lian.Lian_23));
            }

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