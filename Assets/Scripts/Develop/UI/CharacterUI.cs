using DG.Tweening;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class CharacterUI : MonoBehaviour
    {
        [SerializeField] Image _slider;
        private AllyManager _allyManager;
        private Tween _tween;
        private float _duration = 0.5f;

        private void Awake()
        {
            _allyManager = FindAnyObjectByType<AllyManager>();
            _allyManager.GetAllies.OnTakeDamage += OnChanged;
            _allyManager.GetAllies.OnTakeHeal += OnChanged;
        }

        private void OnChanged()
        {
            _tween.Kill();
            float ratio = _allyManager.GetAllies.GetCurrentData.Hp /
                          GameStatics.Characters[(int)_allyManager.GetAllies.GetFirstCharacterType].Hp;
            _tween = _slider.DOFillAmount(ratio, _duration);
        }
    }
}