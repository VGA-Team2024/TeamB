using System.Collections.Generic;
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
        _allyManager = FindObjectOfType<AllyManager>();
        SetAnimationFrag();
    }

    private void SetAnimationFrag()
    {
        _allyManager.GetAllies.OnAttack += () =>
        {
            _animation.SetTrigger(ChantID);
        };

        _allyManager.GetAllies.OnDefenceFailure += () =>
        {
            _animation.SetTrigger(DamageID);
        };
    }
}
