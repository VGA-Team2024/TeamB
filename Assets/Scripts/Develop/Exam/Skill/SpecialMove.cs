using System;
using System.Collections;
using System.Collections.Generic;
using CriWare;
using TeamB.GameSystem.Statics;
using UnityEngine;
using TGS2023.SE;

namespace TeamB.Develop
{
    public class SpecialMove : ISkill
    {
        public event Action OnChantingSkill;
        private CRIAudioManager.SoundPlayer _playback;

        public void Activation(ICharacter mainCharacter, ICharacter character)
        {
            _playback.Stop();
            _playback = CRIAudioManager.SE.Play("SE", nameof(SE.SE_016_Skill_Execute));
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