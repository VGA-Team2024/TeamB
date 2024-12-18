using TeamB.Develop;
using UnityEngine;

public class AllyAnimation : MonoBehaviour
{
    [SerializeField] Animator _animation;
    AllyManager _allyManager;
    int DamageID = Animator.StringToHash("Damage");
    int ChantID = Animator.StringToHash("Chant");

    private void Awake()
    {
        _animation = FindObjectOfType<Animator>();
        _allyManager = FindObjectOfType<AllyManager>();
        SetAnimationFrag();
    }

    private void SetAnimationFrag()
    {
        _allyManager.GetAllies.OnAttack += () =>
        {
            AnimationFragClear();
            _animation.SetBool(ChantID, true);
        };
        _allyManager.GetAllies.OnEndAttack += () =>
        {
            AnimationFragClear();
            _animation.SetBool(ChantID, false);
        };

        _allyManager.GetAllies.OnTakeDamage += () =>
        {
            AnimationFragClear();
            _animation.SetBool(DamageID, true);
        };
        _allyManager.OnEndDamageEffect += () =>
        {
            AnimationFragClear();
            _animation.SetBool(DamageID, false);
        };
    }

    private void AnimationFragClear()
    {
        _animation.SetBool(ChantID, false);
        _animation.SetBool(DamageID, false);
    }
}