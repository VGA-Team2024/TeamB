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

        private void Awake()
        {
            _allyManager = FindAnyObjectByType<AllyManager>();
        }

        public void AddBuffButton()
        {
            _allyManager.AddBuff(buff);
        }
        public void AddDeBuffButton()
        {
            _allyManager.AddBuff(buff);
        }
    }
}
