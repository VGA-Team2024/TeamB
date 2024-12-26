using System;
using UnityEngine;
using UnityEngine.UI;
using TGS2023.SE;

namespace TeamB.Develop
{
    /// <summary>
    /// スキルのボタンを管理するクラス
    /// </summary>
    public class SkillButtonUI : MonoBehaviour
    {
        [SerializeField] SkillType _skillType;
        [SerializeField] Image _skillImage;
        SkillManager _manager;
        private float cost;
        private bool _isChargeComplete;

        private void Awake()
        {
            _manager = FindAnyObjectByType<SkillManager>();
            cost = _manager.SearchSkill(_skillType).cost;
        }

        private void Update()
        {
            if(!_skillImage)
                return;
            if (cost <= _manager.GetCurrentHaveCost)
            {
                
                //クールタイムあけたら一度だけSEを鳴らす
                if (_isChargeComplete == false)
                {
                    CRIAudioManager.SE.Play("SE", nameof(SE.SE_022_Can_SpecialMove));
                    _isChargeComplete = true;
                }
                _skillImage.fillAmount = 0f;
                _skillImage.gameObject.SetActive(false);
            }
            else
            {
                _skillImage.fillAmount = 1f;
                _skillImage.gameObject.SetActive(true);
                _isChargeComplete = false;
            }
        }

        public void ButtonClick()
        {
            _manager.ActivationSkill(_skillType);
            if (cost > _manager.GetCurrentHaveCost)
                CRIAudioManager.SE.Play("SE", nameof(SE.SE_023_Cant_SpecialMove));
        }
    }
}