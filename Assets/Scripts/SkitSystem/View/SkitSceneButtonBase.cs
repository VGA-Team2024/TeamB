using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    public class SkitSceneButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private float _upScale = 1.1f;
        [SerializeField] private float _duration = 0.1f;
        [SerializeField] private Ease _ease = Ease.Linear;
        public event Action OnClick; 

        private void Awake()
        {
            _button.OnClickAsObservable().Subscribe( async _ =>
            {
                await _buttonImage.rectTransform.DOScale(_upScale, _duration).SetEase(_ease).SetLink(gameObject).ToUniTask(cancellationToken:destroyCancellationToken);
                OnClick?.Invoke();
            }).AddTo(this);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            _buttonImage.rectTransform.DOScale(_upScale, _duration).SetEase(_ease).SetLink(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            _buttonImage.rectTransform.DOScale(1, _duration).SetEase(_ease).SetLink(gameObject);
        }
        
        public void LockButton(bool isLock)
        {
            _button.interactable = !isLock;
        }
    }
}

