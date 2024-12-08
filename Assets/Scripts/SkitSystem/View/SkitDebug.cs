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
        [SerializeField] private SkitScenePresenter _skitScenePresenter;
        [SerializeField] private Button _skitDebugButtonPrefab;
        [SerializeField] private GameObject _skitDebugPanel;
        [SerializeField] private RectTransform _skitDebugButtonParent;
        [SerializeField] private RectTransform _classSelectButtonParent;
        [SerializeField] private RectTransform _tutorialButtonParent;
        
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
            
            foreach (Transform child in _classSelectButtonParent)
            {
                Destroy(child.gameObject);
            }
            foreach (var classSelect in _skitScenePresenter.SkitDataLoader.GetAllClassSelectData())
            {
                var button = Instantiate(_skitDebugButtonPrefab, _classSelectButtonParent);
                button.GetComponentInChildren<TMP_Text>().text = classSelect.Id;
                button.onClick.AddListener(() =>
                {
                    SetTestClassSelectId(classSelect.Id);
                    StartSkit();
                });
            }
            
            foreach (Transform child in _tutorialButtonParent)
            {
                Destroy(child.gameObject);
            }
            if (_skitScenePresenter.SkitDataLoader.TryGetTutorialDataById(out var tutorial))
            {
                var button = Instantiate(_skitDebugButtonPrefab, _tutorialButtonParent);
                button.GetComponentInChildren<TMP_Text>().text = "tutorial";
                button.onClick.AddListener(() =>
                {
                    SetTestTutorialId();
                    StartSkit();
                });
            }
        }

        private void SetTestTutorialId()
        {
            if (_skitScenePresenter.SkitDataLoader.TryGetTutorialDataById(out var tutorialData))
            {
                _skitScenePresenter.SkitSystemManager.ResetSkitSceneData();
                _skitScenePresenter.SkitSystemManager.SetSkitSceneData(new SkitContext(SkitContext.ContextType.Tutorial,
                    tutorialData, _skitScenePresenter.SkitFlagData));
            }
        }

        private void SetTestSkitId(string testSkitId)
        {
            if (_skitScenePresenter.SkitDataLoader.TryGetSkitDataById(testSkitId, out var skitData))
            {
                _skitScenePresenter.SkitSystemManager.ResetSkitSceneData();
                _skitScenePresenter.SkitSystemManager.SetSkitSceneData(new SkitContext(SkitContext.ContextType.Skit ,skitData, _skitScenePresenter.SkitFlagData));
            }
        }
        
        private void SetTestClassSelectId(string testSkitId)
        {
            if (_skitScenePresenter.SkitDataLoader.TryGetClassSelectDataById(testSkitId, out var skitData))
            {
                _skitScenePresenter.SkitSystemManager.ResetSkitSceneData();
                _skitScenePresenter.SkitSystemManager.SetSkitSceneData(new SkitContext(SkitContext.ContextType.ClassSelect ,skitData, _skitScenePresenter.SkitFlagData));
            }
        }

        private void StartSkit()
        {
            _skitDebugPanel.SetActive(false);
            _skitScenePresenter.SkitSystemManager.DoSkitSequence().Forget();
        }
    }
}
