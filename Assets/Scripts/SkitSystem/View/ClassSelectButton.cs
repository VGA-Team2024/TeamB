using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class ClassSelectButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _classSelectButtonName;
        [SerializeField] private Button _classSelectButton;
        [SerializeField] private Image _rewardImage;
        
        public Button ClassSelectButtonComponent => _classSelectButton;
        
        public void InitializeClassSelectButton(string className, RewardType rewardType)
        {
            _classSelectButtonName.text = className;
            SetRewardImage(rewardType);
        }
        
        private void SetRewardImage(RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.None:
                    break;
                case RewardType.Intuition:
                    _rewardImage.color = Color.blue;
                    break;
                case RewardType.ReadingComprehension:
                    _rewardImage.color = Color.green;
                    break;
                case RewardType.Concentration:
                    _rewardImage.color = Color.red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardType), rewardType, null);
            }
        }
       
    }
}
