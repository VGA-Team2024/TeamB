using System;
using UnityEngine;
using UnityEngine.Events;

namespace TeamB.Develop
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleCallBack : MonoBehaviour
    {
        [SerializeField] public UnityEvent OnCallBack;

        private void Awake()
        {
            var particle = GetComponent<ParticleSystem>().main;
            
            particle.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            OnCallBack?.Invoke();
        }
    }
}
