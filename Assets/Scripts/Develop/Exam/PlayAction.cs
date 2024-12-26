using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayAction : MonoBehaviour
{
    [SerializeField] public UnityEvent OnPlay = new UnityEvent();
    [SerializeField] public UnityEvent OnEnd = new UnityEvent();

    public void EventPlay()
    {
        OnPlay?.Invoke();
    }

    public void EventEnd()
    {
        OnEnd?.Invoke();
    }
}
