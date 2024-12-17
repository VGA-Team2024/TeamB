using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace TeamB.Develop
{
    public class InstantKill : ISkill
    {
        [SerializeField] float percentageDamage = 50;
        public event Action OnChantingSkill;


        void ISkill.Activation(ICharacter character)
        {
            var rand = UnityEngine.Random.Range(0, 100);
            if (rand <= 10)
            {
                character.TakeDamage(character.GetCurrentData.Hp); //10%の確率で即死(現在HP全部持ってく)
                Debug.Log("即死");
            }
            else
            {
                character.TakeDamage(character.GetCurrentData.Hp / 100 * 50); //90%の確率で現在HPから50％のダメージを与える
                Debug.Log("/90%の確率で現在HPから50％のダメージを与た");
            }
        }
    }
}
