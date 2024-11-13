using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// 魔法ファイアのクラス　：サンプルクラス
    /// これみたいに実装すれば量産できる
    /// </summary>
    public class Fire : ISkill
    {
        [SerializeField] float _damage;
        public event Action OnChantingSkill;

        public void Activation(ICharacter mainCharacter, ICharacter character)
        {
            if (character.GetCurrentData.Hp <= 0)
                return;
            float attackBuffed = mainCharacter.TakeBuff(BuffType.GiveDamage,
                mainCharacter.TakeBuff(BuffType.Attack, mainCharacter.TakeBuff(BuffType.De_GiveDamage, _damage)));
            DebugManager.Log($"{nameof(Fire)}で{character}に{attackBuffed}ダメージ与えた");
            character.TakeDamage(attackBuffed);
            OnChantingSkill?.Invoke();
        }
    }
}