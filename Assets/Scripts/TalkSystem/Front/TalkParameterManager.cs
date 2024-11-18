using System;
using TeamB.GameSystem.Statics;
using UnityEngine;

namespace TeamB.TalkSystem
{
    public class TalkParameterManager : MonoBehaviour
    {
        [SerializeField] private float _allRestTime;
        [SerializeField] private int _clearCount;
        [SerializeField] private RewardType _rewardType;

        private void Awake()
        {
            _allRestTime = 0;
            _clearCount = 0;
            _rewardType = RewardType.None;
        }

        public void AddRestTime(float addTime)
        {
            _allRestTime += addTime;
        }
        
        public void AddClearCount(int addCount)
        {
            _clearCount += addCount;
        }
        
        public void SetRewardType(RewardType rewardType)
        {
            _rewardType = rewardType;
        }

        public void SetReward()
        {
            var charaIndex = (int)GameStatics.NurturingCharacterType;
            var addValue =  _allRestTime / _clearCount;
            switch (_rewardType)
            {
                case RewardType.Intuition:
                    GameStatics.Characters[charaIndex].MagicATK += addValue;
                    break;
                case RewardType.ReadingComprehension:
                    GameStatics.Characters[charaIndex].ChantingSpeed += addValue;
                    break;
                case RewardType.Concentration:
                    GameStatics.Characters[charaIndex].HitRate += addValue;
                    break;
                case RewardType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
