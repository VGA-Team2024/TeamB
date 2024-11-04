using System;
using System.Collections;
using System.Collections.Generic;
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

        public void Activation(ICharacter character)
        {
            if (character.GetCurrentData.Hp <= 0)
                return;
            DebugManager.Log($"{nameof(Fire)}で{character}に{_damage}ダメージ与えた");
            character.TakeDamage(_damage);
            OnChantingSkill?.Invoke();
        }
    }
}