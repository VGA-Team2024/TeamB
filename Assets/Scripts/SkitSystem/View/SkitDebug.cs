using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitDebug : MonoBehaviour
    {
        [SerializeField] private SkitSystemManager _skitSystemManager;
        [SerializeField] private SkitScenePresenter _skitScenePresenter;
        [SerializeField] private RectTransform _skitDebugButtonParent;
        [SerializeField] private Button _skitDebugButtonPrefab;
        [SerializeField] private GameObject _skitDebugPanel;
        private ISkitDataLoader _skitDataLoader;
        
        private void Start()
        {
            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ =>
                {
                    _skitDebugPanel.SetActive(!_skitDebugPanel.activeSelf);
                    if (_skitDebugPanel.activeSelf)
                    {
                        SetDebugPanel();
                    }
                }).AddTo(this);
            _skitDebugPanel.SetActive(false);
        }

        private void SetDebugPanel()
        {
            var skitData = _skitScenePresenter.SkitDataLoader.GetAllSkitData();
            foreach (Transform child in _skitDebugButtonParent)
            {
                Destroy(child.gameObject);
            }
            foreach (var skit in skitData)
            {
                var button = Instantiate(_skitDebugButtonPrefab, _skitDebugButtonParent);
                button.GetComponentInChildren<TMP_Text>().text = skit.Id;
                button.onClick.AddListener(() =>
                {
                    SetTestSkitId(skit.Id);
                    StartSkit();
                });
            }
        }
        
        // Start is called before the first frame update
        private void SetTestSkitId(string testSkitId)
        {
            if (_skitDataLoader.TryGetSkitData(testSkitId, out var skitData))
            {
                _skitSystemManager.SetSkitSceneData(new SkitContext(SkitContext.ContextType.Skit ,skitData));
            }
        }

        private void StartSkit()
        {
            _skitDebugPanel.SetActive(false);
            _skitSystemManager.DoSkitSequence().Forget();
        }
    }
}
