using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ポーズ管理クラス
/// </summary>
public class PoseManager : MonoBehaviour
{
    public event Action OnInPose;
    public event Action OnOutPose;

    private bool _isInPose;

    public bool GetIsInPose => _isInPose;

    public void StartPose()
    {
        if (_isInPose)
            return;
        OnInPose?.Invoke();
        _isInPose = true;
    }

    public void StopPose()
    {
        if (!_isInPose)
            return;
        OnOutPose?.Invoke();
        _isInPose = false;
    }
}

public interface IPoseObject
{
    public void StartPose();
    public void EndPose();
}