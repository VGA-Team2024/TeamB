using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB.Develop
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleCallBack : MonoBehaviour
    {
        public event Action OnCallBack;

        private void Awake()
        {
            var particle = GetComponent<ParticleSystem>().main;
            
            particle.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            OnCallBack?.Invoke();
            Debug.Log("Particle call back stopped");
        }
    }
}
