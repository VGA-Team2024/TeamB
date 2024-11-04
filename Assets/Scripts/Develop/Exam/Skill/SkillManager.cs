using System;
using TeamB.Data;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// スキルを管理するクラス
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private SkillData[] skillData;
        [SerializeField] private float _maxCost;

        private EnemyManager _enemyManager;
        private AllyManager _allyManager;
        private float _currentCost = 10;
        public SkillData[] GetSkillData => skillData;
        public float GetCurrentCost => _currentCost;


        [Serializable]
        public class SkillData
        {
            [SerializeField] public ExamState _examState;
            [SerializeField] public SkillState[] _skillState;

            [Serializable]
            public class SkillState
            {
                public SkillType SkillType;
                [SerializeReference, SubclassSelector] public ISkill Skill;
                public Target Target;
                public float Cost;
            }
        }

        private void Awake()
        {
            _enemyManager = FindAnyObjectByType<EnemyManager>();
            _allyManager = FindAnyObjectByType<AllyManager>();
        }

        /// <summary>
        /// 対象を探す
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ICharacter TargetSelect(Target target)
        {
            switch (target)
            {
                case Target.Character:
                    return _allyManager.GetAllies;
                case Target.Enemy:
                    return _enemyManager.GetCurrentEnemyData;
                default:
                    return null;
            }
        }
    }
    
    /// <summary>
    /// スキルを実装する時に継承するクラス
    /// </summary>
    public interface ISkill
    {
        public event Action OnChantingSkill;
        /// <summary>
        /// スキル発動
        /// </summary>
        /// <param name="character"></param>
        public void Activation(ICharacter character);
    }

    public enum SkillType
    {
        Fire,
        Blizzard,
        Wind,
        Water,
        Heal,
        Barrier,
        None
    }

    public enum Target
    {
        Character,
        Enemy,
        None
    }
}