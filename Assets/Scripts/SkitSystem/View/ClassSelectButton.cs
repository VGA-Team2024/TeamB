using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class ClassSelectButton : SkitSceneButtonBase
    {
        [SerializeField] private TMP_Text _classSelectButtonName;
        [SerializeField] private Image _rewardImage;
        [SerializeField] private Sprite _intuitionSprite;
        [SerializeField] private Sprite _readingComprehensionSprite;
        [SerializeField] private Sprite _concentrationSprite;
        
        
        public void InitializeClassSelectButton(string className, RewardType rewardType)
        {
            _classSelectButtonName.text = className;
            SetRewardImage(rewardType);
            OnClick += () => SkitRewardManager.Instance.SetRewardType(rewardType);
        }
        
        private void SetRewardImage(RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.None:
                    break;
                case RewardType.Intuition: 
                    _rewardImage.sprite = _intuitionSprite;
                    break;
                case RewardType.ReadingComprehension:
                    _rewardImage.sprite = _readingComprehensionSprite;
                    break;
                case RewardType.Concentration:
                    _rewardImage.sprite = _concentrationSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardType), rewardType, null);
            }
        }
       
    }
}
