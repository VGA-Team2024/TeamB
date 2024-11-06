using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TeamB.ConversationSystem
{
    public class ConvesationFade : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage = null;
        [SerializeField] private float duration = 1.0f;

        public void FadeIn()
        {
            _fadeImage.color = new Color(0, 0, 0, 1);
            _fadeImage.DOFade(0, 1.0f);
            _fadeImage.DOFade(1, 10);
        }

        void Start()
        {
            // 画像を1秒かけて透明にする (alpha: 0)
            //_fadeImage.DOFade(0f, duration);
        }
    }
}