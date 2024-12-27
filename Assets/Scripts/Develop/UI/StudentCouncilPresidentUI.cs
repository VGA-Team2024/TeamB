using DG.Tweening;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class StudentCouncilPresidentUI : MonoBehaviour
    {
        [SerializeField] Image _slider;
        private EnemyManager _enemyManager;
        private Exam _exam;
        private float _duration = 0.5f;
        private float _before;
        Tween _tween;

        private void Awake()
        {
            _enemyManager = FindAnyObjectByType<EnemyManager>();
            _exam = FindAnyObjectByType<Exam>();
            _enemyManager.GetCurrentEnemyData.OnTakeDamage += OnChanged;
        }

        private void OnChanged()
        {
            if (GameStatics.ExamState == ExamState.SecondExam)
            {
                _tween.Kill();
                float ratio = Mathf.Clamp01(_enemyManager.GetCurrentEnemyData.GetCurrentData.Hp /
                              GameStatics.Characters[(int)_enemyManager.GetCurrentEnemyData.GetFirstCharacterType].Hp);
                
                _tween = _slider.DOFillAmount(ratio, _duration);
            }
            else
            {
                _tween.Kill();
                float ratio = _enemyManager.GetCurrentEnemyData.GetCurrentData.Hp /
                              GameStatics.Characters[(int)_enemyManager.GetCurrentEnemyData.GetSecondCharacterType].Hp;
                
                _tween = _slider.DOFillAmount(ratio, _duration);
            }
        }
    }
}