using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class SkilUI : MonoBehaviour
    {
        [SerializeField] Image _slider;
        SkillManager manager;

        private void Awake()
        {
            manager = FindAnyObjectByType<SkillManager>();
            manager.OnCostRecovery += CostSliderChanged;
        }

        public void CostSliderChanged()
        {
            _slider.fillAmount = manager.GetCurrentHaveCost / manager.GetMaxCost;
        }
    }
}
