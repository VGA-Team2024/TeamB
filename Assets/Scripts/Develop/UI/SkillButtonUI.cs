using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamB.Develop
{
    /// <summary>
    /// スキルのボタンを管理するクラス
    /// </summary>
    public class SkillButtonUI : MonoBehaviour
    {
        [SerializeField] SkillType _skillType;
        private (ISkill skill, Target target, float cost) info;
        SkillManager manager;

        private void Awake()
        {
            manager = FindAnyObjectByType<SkillManager>();
            info = SearchSkill(_skillType);
        }

        public void ButtonClick()
        {
            if (manager.GetCurrentHaveCost >= info.cost)
            {
                info.skill.Activation(manager.TargetSelect(info.target));
                manager.CostDecrease(info.cost);
            }
        }


        /// <summary>
        /// スキル種類からスキル、対象、コストを得る
        /// </summary>
        /// <param name="skillType"></param>
        /// <returns></returns>
        (ISkill skill, Target target, float cost) SearchSkill(SkillType skillType)
        {
            ISkill skill;
            Target target;
            float cost;
            for (int i = 0; i < manager.GetSkillData.Length; i++)
            {
                for (int n = 0; n < manager.GetSkillData[i]._skillState.Length; n++)
                {
                    if (manager.GetSkillData[i]._examState == GameStatics.ExamState &&
                        manager.GetSkillData[i]._skillState[n].SkillType == skillType)
                    {
                        skill = manager.GetSkillData[i]._skillState[n].Skill;
                        target = manager.GetSkillData[i]._skillState[n].Target;
                        cost = manager.GetSkillData[i]._skillState[n].Cost;
                        return (skill, target, cost);
                    }
                }
            }

            return (null, Target.None, 0f);
        }
    }
}