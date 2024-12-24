using System;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.Playables;

namespace TeamB.Develop
{
    /// <summary>
    /// スキルを管理するクラス
    /// </summary>
    public class SkillManager : MonoBehaviour, IExam
    {
        #region SerializedFields

        [SerializeField]
        private SkillData[] _skillData;

        [SerializeField]
        private float _maxCost;

        [SerializeField, Header("1秒に回復するコストの量")]
        private float _recoveryCost;

        [SerializeField]
        private PlayableDirector _playableDirector;

        #endregion

        #region Privates

        private EnemyManager _enemyManager;
        private AllyManager _allyManager;
        private Exam _exam;
        private PoseManager _poseManager;
        private float _currentHaveCost;

        #endregion

        #region Actions

        public event Action OnCostRecovery;

        #endregion

        #region Properties

        public SkillData[] GetSkillData => _skillData;
        public float GetCurrentHaveCost => _currentHaveCost;
        public float GetMaxCost => _maxCost;

        #endregion

        [Serializable]
        public class SkillData
        {
            [SerializeField]
            public ExamState _examState;

            [SerializeField]
            public SkillState[] _skillState;

            [Serializable]
            public class SkillState
            {
                public SkillType SkillType;

                [SerializeReference, SubclassSelector]
                public ISkill Skill;

                public Target Target;
                public float Cost;
            }
        }

        private void Awake()
        {
            _enemyManager = FindAnyObjectByType<EnemyManager>();
            _allyManager = FindAnyObjectByType<AllyManager>();
            _exam = FindAnyObjectByType<Exam>();
            _exam.OnExamStarted += OnStartExam;
            _exam.OnExamEnded += OnEndExam;
        }

        public void ActivationSkill(SkillType skillType)
        {
            if (_poseManager == null)
                _poseManager = FindAnyObjectByType<PoseManager>();
            var info = SearchSkill(skillType);
            if (GetCurrentHaveCost >= info.cost)
            {
                info.skill.Activation(_allyManager.GetAllies, TargetSelect(info.target));
                CostDecrease(info.cost);
                
                
            }
        }

        /// <summary>
        /// スキル種類からスキル、対象、コストを得る
        /// </summary>
        /// <param name="skillType"></param>
        /// <returns></returns>
        public (ISkill skill, Target target, float cost) SearchSkill(SkillType skillType)
        {
            ISkill skill;
            Target target;
            float cost;
            for (int i = 0; i < GetSkillData.Length; i++)
            {
                for (int n = 0; n < GetSkillData[i]._skillState.Length; n++)
                {
                    if (GetSkillData[i]._examState == GameStatics.ExamState &&
                        GetSkillData[i]._skillState[n].SkillType == skillType)
                    {
                        skill = GetSkillData[i]._skillState[n].Skill;
                        target = GetSkillData[i]._skillState[n].Target;
                        cost = GetSkillData[i]._skillState[n].Cost;
                        return (skill, target, cost);
                    }
                }
            }

            return (null, Target.None, 0f);
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

        public void CostDecrease(float consumptionCost)
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

        public void OnStartExam()
        {
            _exam.OnExamUpdated += CostRecovery;
        }

        public void OnEndExam()
        {
            _exam.OnExamStarted -= OnStartExam;
            _exam.OnExamUpdated -= CostRecovery;
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
        public void Activation(ICharacter mainCharacter, ICharacter character);
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