using TeamB.GameSystem.Statics;
using UnityEngine;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーンにおいてプレイヤーのパラメータ上昇を管理するクラス
    /// 会話シーンでしか使わないのでシングルトンで実装し、シーンをまたいだら破棄される
    /// </summary>
    public class SkitRewardManager : MonoBehaviour
    {
        private static SkitRewardManager _instance;

        public static SkitRewardManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<SkitRewardManager>();
                if (_instance != null) return _instance;
                var singletonObject = new GameObject(nameof(SkitRewardManager));
                _instance = singletonObject.AddComponent<SkitRewardManager>();

                return _instance;
            }
        }
        private RewardType _currentRewardType = RewardType.Concentration;
        private float _remainTimeSum;
        private int _correctCount;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void SetRewardType(RewardType rewardType)
        {
            _currentRewardType = rewardType;
        }
        
        //ステータス上昇値の計算式	→	Val = 回答残り時間の合計 / 正答数 	
        public void AddRewardValue(float remainTime)
        {
            _remainTimeSum += remainTime;
            _correctCount++;
            AddStatus();
        }
        
        public void AddStatus()
        {
            if (_correctCount == 0)
            {
                return;
            }
            var rewardValue = _remainTimeSum / _correctCount;
            var key = (int)GameStatics.NurturingCharacterType;
            if (!GameStatics.Characters.ContainsKey(key)) return;
            CRIAudioManager.SE.Play(SkitSoundKey.SeSheetName, SkitSoundKey.ParameterUp);
            switch (_currentRewardType)
            {
                case RewardType.Intuition:
                    //直観力
                    GameStatics.Characters[(int) GameStatics.NurturingCharacterType].MagicATK += rewardValue;
                    break;
                case RewardType.ReadingComprehension:
                    //読解力
                    GameStatics.Characters[(int) GameStatics.NurturingCharacterType].ChantingSpeed += rewardValue;
                    break;
                case RewardType.Concentration:
                    //集中力
                    GameStatics.Characters[(int) GameStatics.NurturingCharacterType].HitRate += rewardValue;
                    break;
            }
        }
    }
}
