using System;
using System.Collections.Generic;
using System.Linq;
using TeamB.Develop;
using UnityEngine;

namespace TeamB.GameSystem
{
    /// <summary>
    ///     全バフの情報があるクラス
    /// </summary>
    public class BuffContainer : MonoBehaviour
    {
        [SubclassSelector, SerializeReference] private IBuff[] _buffDatas;

        public IBuff[] GetBuffDatas => _buffDatas;

        public IBuff GetBuffData(int type)
        {
            BuffType buffType = (BuffType)type;
            IBuff[] buffDatas = _buffDatas.Where(x => x.GetBuffType == buffType).ToArray();
            if (buffDatas == null || buffDatas.Length == 0)
            {
                DebugManager.Log($"{_buffDatas.Length} buffs are empty!");
                return null;
            }

            return buffDatas[0];
        }
    }

    /// <summary>
    ///     魔法攻撃力バフ
    /// </summary>
    [Serializable]
    public class MagicAttackBuff : IBuff
    {
        [SerializeField] private string _buffName;
        [SerializeField] private BuffType _buffType;
        [SerializeField] private CalculationMethod _calculationMethod;
        [SerializeField] private float _duration;
        [SerializeField] private float _value;
        [SerializeField] private GameObject _effect;
        [SerializeField] private int _useLimit;
        private int _useCount;
        private bool _isTimed;
        private float _timer;
        public int GetUseLimit => _useLimit;
        public int GetUseCount => _useCount;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;


        public bool Timer(float deltaTime)
        {
            if (_duration == -1)
                return false;
            _isTimed = _timer >= _duration;
            _timer += deltaTime;
            return _isTimed;
        }
        
        public int CountUpLimit()
        {
            _useCount++;
            return _useCount;
        }
    }

    /// <summary>
    ///     詠唱速度バフ
    /// </summary>
    [Serializable]
    public class ChantingSpeedBuff : IBuff
    {
        [SerializeField] private string _buffName;
        [SerializeField] private BuffType _buffType;
        [SerializeField] private CalculationMethod _calculationMethod;
        [SerializeField] private float _duration;
        [SerializeField] private float _value;
        [SerializeField] private GameObject _effect;
        [SerializeField] private int _useLimit;
        private bool _isTimered;
        private int _useCount;
        public int GetUseCount => _useCount;
        private float _timer;
        public int GetUseLimit => _useLimit;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;

        public bool Timer(float deltaTime)
        {
            if (_duration == -1)
                return false;
            _isTimered = _timer >= _duration;
            _timer += deltaTime;
            return _isTimered;
        }
        
        public int CountUpLimit()
        {
            _useCount++;
            return _useCount;
        }
    }


    /// <summary>
    ///     命中率バフ
    /// </summary>
    [Serializable]
    public class HitRateBuff : IBuff
    {
        [SerializeField] private string _buffName;
        [SerializeField] private BuffType _buffType;
        [SerializeField] private CalculationMethod _calculationMethod;
        [SerializeField] private float _duration;
        [SerializeField] private float _value;
        [SerializeField] private GameObject _effect;
        [SerializeField] private int _useLimit;
        private bool _isTimed;
        private float _timer;
        public int GetUseLimit => _useLimit;
        private int _useCount;
        public int GetUseCount => _useCount;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;

        public bool Timer(float deltaTime)
        {
            if (_duration == -1)
                return false;
            _isTimed = _timer >= _duration;
            _timer += deltaTime;
            return _isTimed;
        }
        
        public int CountUpLimit()
        {
            _useCount++;
            return _useCount;
        }
    }

    /// <summary>
    ///     与えるダメージバフ
    /// </summary>
    [Serializable]
    public class GiveDamageBuff : IBuff
    {
        [SerializeField] private string _buffName;
        [SerializeField] private BuffType _buffType;
        [SerializeField] private CalculationMethod _calculationMethod;
        [SerializeField] private float _duration;
        [SerializeField] private float _value;
        [SerializeField] private GameObject _effect;
        [SerializeField] private int _useLimit;
        private bool _isTimered;
        private int _useCount;
        private float _timer;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public int GetUseLimit => _useLimit;
        public int GetUseCount => _useCount;

        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;

        public bool Timer(float deltaTime)
        {
            if (_duration == -1)
                return false;
            _isTimered = _timer >= _duration;
            _timer += deltaTime;
            return _isTimered;
        }

        public int CountUpLimit()
        {
            _useCount++;
            return _useCount;
        }
    }

    /// <summary>
    ///     バフが持つべきインターフェース
    /// </summary>
    public interface IBuff
    {
        public string GetBuffName { get; }
        public BuffType GetBuffType { get; }
        public CalculationMethod GetCalculationMethod { get; }
        public int GetUseLimit { get; }
        public int GetUseCount { get; }
        public float GetDuration { get; }
        public float GetValue { get; }
        public GameObject GetEffect { get; }

        public bool Timer(float deltaTime);
        public int CountUpLimit();
    }

    /// <summary>
    ///     バフの種類
    /// </summary>
    public enum BuffType
    {
        None,
        Health,
        Attack,
        CastingSpeed,
        HitRate,
        GiveDamage,
        De_GiveDamage
    }

    /// <summary>
    ///     バフの計算方法
    /// </summary>
    public enum CalculationMethod
    {
        None,
        Multiplication,
        Addition
    }
}