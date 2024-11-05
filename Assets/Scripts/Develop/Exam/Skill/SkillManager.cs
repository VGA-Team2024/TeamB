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

        [SerializeField, Header("1秒に回復するコストの量")]
        private float _recoveryCost;

        private EnemyManager _enemyManager;
        private AllyManager _allyManager;
        private Exam _exam;
        private float _currentHaveCost;

        public event Action OnCostRecovery;

        public SkillData[] GetSkillData => skillData;
        public float GetCurrentHaveCost => _currentHaveCost;
        public float GetMaxCost => _maxCost;


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
            _exam = FindAnyObjectByType<Exam>();
            _exam.OnExamUpdated += CostRecovery;
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

        public void CostDecrease(float　consumptionCost)
        {
            _currentHaveCost -= consumptionCost;
        }

        private void CostRecovery(float deltaTime)
        {
            if (_currentHaveCost >= _maxCost)
                return;
            _currentHaveCost += _recoveryCost * deltaTime;
            OnCostRecovery?.Invoke();

            if (_currentHaveCost >= _maxCost)
            {
                _currentHaveCost = _maxCost;
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