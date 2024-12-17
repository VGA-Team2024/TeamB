using System;
using TeamB.GameSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamB.Develop
{
    public class BuffButtonUI : MonoBehaviour
    {
        [SerializeField] private BuffType buff;
        private Color _color = Color.gray;
        private AllyManager _allyManager;
        private BuffContainer _buffContainer;
        private BuffUI _buffUI;
        private IBuff buffData;

        public event Action OnAddBuff;
        public event Action OnLimit;

        private void Start()
        {
            _allyManager = FindAnyObjectByType<AllyManager>();
            _buffContainer = FindAnyObjectByType<BuffContainer>();
            _buffUI = FindAnyObjectByType<BuffUI>();
        }

        public void AddBuffButton()
        {
            buffData = _buffContainer.GetBuffData((int)buff);
            if (buffData?.GetUseLimit <= buffData?.GetUseCount)
                return;
            _allyManager.AddBuff(buff);
            _buffUI.ReturnBuffUI();
            buffData?.CountUpLimit();
            OnAddBuff?.Invoke();
            if (buffData?.GetUseLimit <= buffData?.GetUseCount)
            {
                OnLimit?.Invoke();
                Destroy(gameObject);
            }
        }

        public void AddDeBuffButton()
        {
            buffData = _buffContainer.GetBuffData((int)buff);
            if (buffData?.GetUseLimit <= buffData?.GetUseCount)
                return;
            _allyManager.AddBuff(buff);
            _buffUI.ReturnBuffUI();
            buffData?.CountUpLimit();
            OnAddBuff?.Invoke();
            if (buffData?.GetUseLimit <= buffData?.GetUseCount)
            {
                OnLimit?.Invoke();
                Destroy(this.gameObject);
            }
        }
    }
}