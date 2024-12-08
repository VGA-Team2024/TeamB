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
        [SerializeField] private Sprite _intuitionSprite;
        [SerializeField] private Sprite _readingComprehensionSprite;
        [SerializeField] private Sprite _concentrationSprite;
        
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
