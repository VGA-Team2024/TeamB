using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーンのフェードを管理するクラス
    /// </summary>
    public class SkitViewFade : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private GameObject _loadingText;
        [SerializeField] private float _fadeTime = 1.0f;

        public async UniTask FadeInAsync(bool immediate = false)
        {
            if (immediate)
            {
                _fadeImage.color = new Color(0, 0, 0, 0);
                await _fadeImage.DOFade(1, 0).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
            else
            {
                _fadeImage.color = new Color(0, 0, 0, 0);
                await _fadeImage.DOFade(1, _fadeTime).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
        }
        
        public async UniTask FadeOutAsync(bool immediate = false)
        {
            if (immediate)
            {
                _fadeImage.color = new Color(0, 0, 0, 1);
                await _fadeImage.DOFade(0, 0).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
            else
            {
                _fadeImage.color = new Color(0, 0, 0, 1);
                await _fadeImage.DOFade(0, _fadeTime).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
        }
    }
}
