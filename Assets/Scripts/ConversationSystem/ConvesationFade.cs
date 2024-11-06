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
            _fadeImage.DOFade(0f, duration);
        }

        public void FadeOut()
        {
            _fadeImage.color = new Color(0, 0, 0, 0);
            _fadeImage.DOFade(1f, duration);
        }
    }
}