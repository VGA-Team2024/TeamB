using System;
using System.Linq;
using UnityEngine;

namespace MockUp
{
    public class WackAMoleManager : MonoBehaviour
    {
        private TimeManager _timeManager;
        private MoleUI _moleUI;
        private MoleController _moleController;

        public event Action OnGameStart;
        public event Action OnGameEnd;


        public void GameStart()
        {
            OnGameStart?.Invoke();
            Initialized();
        }

        public void GameEnd()
        {
            OnGameEnd?.Invoke();
        }

        public void Initialized()
        {
            _timeManager = FindAnyObjectByType<TimeManager>();
            _timeManager.OnLimit += GameEnd;
            GetComponents<IInitialized>().ToList().ForEach(x => x.Initialize());
        }

        private void OnDisable()
        {
            if (_timeManager)
                _timeManager.OnLimit -= GameEnd;
        }
    }

    public interface IInitialized
    {
        public void Initialize();
    }
}