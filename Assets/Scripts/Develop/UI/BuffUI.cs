using DG.Tweening;
using TeamB.GameSystem;
using UISystem;
using UnityEngine;

namespace TeamB.Develop
{
    public class BuffUI : UIView
    {
        [SerializeField] RectTransform _buffPanelPrefab;
        [SerializeField] Vector3 _buffPanelStartOffset;
        [SerializeField] Vector3 _buffPanelEndOffset;
        private PoseManager _poseManager;
        private WaveManager _waveManager;
        
        protected override void AwakeCall()
        {
            _poseManager = FindAnyObjectByType<PoseManager>();
            _waveManager = FindAnyObjectByType<WaveManager>();
            _waveManager.OnNextWave += ComeBufferUI;
        }

        public void ComeBufferUI()
        {
            if(!_poseManager)
                _poseManager = FindAnyObjectByType<PoseManager>();
            _poseManager.StartPose();
            _buffPanelPrefab.DOLocalMove(_buffPanelEndOffset, 1f);
        }

        public void ReturnBuffUI()
        {
            if(!_poseManager)
                _poseManager = FindAnyObjectByType<PoseManager>();
            _poseManager.StopPose();
            _buffPanelPrefab.DOLocalMove(_buffPanelStartOffset, 1f);
        }
    }
}
