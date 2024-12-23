using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using UnityEngine;

public class ParticlePlay : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles;
    [SerializeField] List<Animator> animators;

    public void Play(int examState)
    {
        if(GameStatics.ExamState != (ExamState)examState)
            return;
        foreach (var par in particles)
        {
            par.Play();
        }

        foreach (var anim in animators)
        {
            anim.SetTrigger("Play");
        }
    }
}
