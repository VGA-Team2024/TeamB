using System;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// ウェーブ管理
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        private int _currentWave = 1;

        private Exam _exam;

        public event Action OnNextWave;
        public int GetCurrentWave => _currentWave;

        private void Awake()
        {
            _exam = FindObjectOfType<Exam>();
        }

        /// <summary>
        /// ウェーブを進める
        /// </summary>
        public void NextWave()
        {
            _currentWave++;
            OnNextWave?.Invoke();
        }
    }
}
