using System;
using UnityEngine;

namespace TeamB.InGameData.Data
{
    public class SoulManager : MonoBehaviour
    {
        private float _currentSoul;
        public event Action OnAddSoul;

        public float GetCurrentSoul => _currentSoul;

        public void AddSoul(float newSoul)
        {
            _currentSoul += newSoul;
            OnAddSoul?.Invoke();
        }
    }
}