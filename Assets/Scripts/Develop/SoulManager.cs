using System;
using System.Linq;
using UnityEngine;

namespace TeamB.InGameData.Data
{
    public class SoulManager : MonoBehaviour
    {
        [SerializeField] SoulData[] soulData;
        private float _currentSoul;
        public event Action OnAddSoul;
        public event Action OnInstanceSoul;
        
        public SoulData[] GetSoulData => soulData;
        

        public float GetCurrentSoul => _currentSoul;

        public void AddSoul(float newSoul)
        {
            _currentSoul += newSoul;
            OnAddSoul?.Invoke();
        }

        /// <summary>
        /// 魂生成
        /// </summary>
        /// <param name="soulData"></param>
        public float InstanceSoul(SoulSize size)
        {
            var soul = soulData.Where(x => x.SoulSize == size).ToArray();
            OnInstanceSoul?.Invoke();
            return soul[0].AddSoulValue;
        }

        /// <summary>
        /// 魂生成(ランダム)
        /// </summary>
        /// <param name="soulData"></param>
        public float InstanceRandomSoul()
        {
            int rand  = UnityEngine.Random.Range(0, Enum.GetValues(typeof(SoulSize)).Length-1);
            var soul = soulData.Where(x => x.SoulSize == (SoulSize)rand).ToArray();
            float value = soul[0].AddSoulValue;
            OnInstanceSoul?.Invoke();
            return value;
        }
        
        [Serializable]
        public class SoulData
        {
            public SoulSize SoulSize;
            public float AddSoulValue;
        }
    }

    public enum SoulSize
    {
        Small,
        Medium,
        Big,
        None
    }
}