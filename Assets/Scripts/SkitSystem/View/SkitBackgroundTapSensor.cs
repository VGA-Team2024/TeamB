using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamB.SkitSystem
{
    public class SkitBackgroundTapSensor : MonoBehaviour, IPointerClickHandler
    {
        public event Action OnBackgroundTap; 
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Background tapped");
            OnBackgroundTap?.Invoke();
            OnBackgroundTap = null;
        }
    }
}
