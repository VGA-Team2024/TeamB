using UnityEngine;

namespace TeamB.Develop
{
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] Animator _animation;
        EnemyManager _allyManager;
        int DamageID = Animator.StringToHash("Damage");
        int ChantID = Animator.StringToHash("Chant");

        private void Awake()
        {
            _allyManager = FindObjectOfType<EnemyManager>();
            SetAnimationFrag();
        }

        private void SetAnimationFrag()
        {
            _allyManager.GetCurrentEnemyData.OnAttack += () =>
            {
                _animation.SetTrigger(ChantID);
            };

            _allyManager.GetCurrentEnemyData.OnTakeDamage += () =>
            {
                _animation.SetTrigger(DamageID);
            };
        }
    }
}