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

    public void StartPose()
    {
        OnInPose?.Invoke();
    }

    public void StopPose()
    {
        OnOutPose?.Invoke();
    }
}

/// <summary> ポーズ時に処理するクラスに継承する </summary>
public interface IPose
{
    public void InPose();
    public void OutPose();
}

/// <summary></summary>
public interface IInitialized
{
    public void Initialize();
}