using System;
using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem;
using UnityEngine;

namespace TeamB.Develop
{
    public class BuffButtonUI : MonoBehaviour
    {
        [SerializeField] BuffType buff;
        
        AllyManager _allyManager;
        BuffUI _buffUI;
        
        private void Awake()
        {
            _allyManager = FindAnyObjectByType<AllyManager>();
            _buffUI = FindAnyObjectByType<BuffUI>();
        }

        public void AddBuffButton()
        {
            Debug.Log("Adding buff button");
            _allyManager.AddBuff(buff);
            _buffUI.ReturnBuffUI();
        }

        public void AddDeBuffButton()
        {
            _allyManager.AddBuff(buff);
            _buffUI.ReturnBuffUI();
        }
    }
}