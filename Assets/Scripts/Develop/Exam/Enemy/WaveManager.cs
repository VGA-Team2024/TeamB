using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using UnityEngine;

namespace TeamB.Develop
{
    /// <summary>
    /// ウェーブ管理
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        private int _currentWave = 0;

        private Exam _exam;

        public event Action OnNextWave;
        public int GetCurrentWave => _currentWave;

        private void Awake()
        {
            _exam = FindObjectOfType<Exam>();
            _exam.OnExamStarted += NextWave;
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
