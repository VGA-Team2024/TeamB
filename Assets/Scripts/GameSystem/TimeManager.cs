using System;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// 時間管理クラス
/// </summary>
public class TimeManager
{
    private CancellationTokenSource tokenSource = new CancellationTokenSource();
    public event Action OnStart;
    public event Action<int> OnUpdate;
    public event Action OnLimit;


    /// <summary>
    /// タイマー機能
    /// </summary>
    public async UniTask TimerUpdateAsync(float timeLimit)
    {
        OnStart?.Invoke();
        for (int i = 0; i < timeLimit * 100 && tokenSource != null; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f), cancellationToken: tokenSource.Token);
            OnUpdate?.Invoke(i);
        }

        OnLimit?.Invoke();
    }

    public void Cancel()
    {
        tokenSource.Cancel();
        tokenSource = null;
    }
}