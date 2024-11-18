using System;
using System.Linq;
using TeamB.Data;
using TeamB.Develop;
using UnityEngine;

namespace TeamB.GameSystem
{
    /// <summary>
    /// 全バフの情報があるクラス
    /// </summary>
    public class BuffContainer : MonoBehaviour
    {
        [SubclassSelector, SerializeReference] private IBuff[] _buffDatas;

        public IBuff[] GetBuffDatas => _buffDatas;

        public IBuff GetBuffData(int type)
        {
            BuffType buffType = (BuffType)type;
            IBuff[] buffDatas = _buffDatas.Where(x => x.GetBuffType == buffType).ToArray();
            return buffDatas[0];
        }
    }

    /// <summary>
    /// 魔法攻撃力バフ
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
        private float _timer;
        bool _isTimered;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;

        public bool Timer(float deltaTime)
        {
            if(_duration == -1)
                return false;
            _isTimered = _timer >= _duration;
            _timer += deltaTime;
            return _isTimered;
        }
    }

    /// <summary>
    /// 詠唱速度バフ
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
        private float _timer;
        bool _isTimered;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;

        public bool Timer(float deltaTime)
        {
            if(_duration == -1)
                return false;
            _isTimered = _timer >= _duration;
            _timer += deltaTime;
            return _isTimered;
        }
    }


    /// <summary>
    /// 命中率バフ
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
        private float _timer;
        bool _isTimered;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;

        public bool Timer(float deltaTime)
        {
            if(_duration == -1)
                return false;
            _isTimered = _timer >= _duration;
            _timer += deltaTime;
            return _isTimered;
        }
    }

    /// <summary>
    /// 与えるダメージバフ
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
        private float _timer;
        bool _isTimered;

        public string GetBuffName => _buffName;
        public BuffType GetBuffType => _buffType;
        public CalculationMethod GetCalculationMethod => _calculationMethod;
        public float GetDuration => _duration;
        public float GetValue => _value;
        public GameObject GetEffect => _effect;

        public bool Timer(float deltaTime)
        {
            if(_duration == -1)
                return false;
            _isTimered = _timer >= _duration;
            _timer += deltaTime;
            return _isTimered;
        }
    }

    /// <summary>
    /// バフが持つべきインターフェース
    /// </summary>
    public interface IBuff
    {
        public string GetBuffName { get; }
        public BuffType GetBuffType { get; }
        public CalculationMethod GetCalculationMethod { get; }
        public float GetDuration { get; }
        public float GetValue { get; }
        public GameObject GetEffect { get; }

        public bool Timer(float deltaTime);
    }

    /// <summary>
    /// バフの種類
    /// </summary>
    public enum BuffType
    {
        None,
        Health,
        Attack,
        CastingSpeed,
        HitRate,
        GiveDamage,
        De_GiveDamage,
    }

    /// <summary>
    /// バフの計算方法
    /// </summary>
    public enum CalculationMethod
    {
        None,
        Multiplication,
        Addition
    }
}