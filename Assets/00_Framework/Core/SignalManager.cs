using System;
using System.Collections.Generic;
using Framework.Utilities;
using UnityEngine;

public class SignalManager : Singleton<SignalManager>
{
    protected override void InitializeManager()
    {
        Debug.Log("SignalManager Initialized");
    }

    // 신호와 델리게이트를 제네릭으로 관리
    private readonly Dictionary<string, Delegate> _signals = new();

    /// <summary>
    /// 신호를 연결합니다.
    /// </summary>
    public void ConnectSignal<T>(string signalKey, Action<T> action)
    {
        if (!_signals.TryAdd(signalKey, action))
        {
            // 기존 신호에 연결
            _signals[signalKey] = Delegate.Combine(_signals[signalKey], action);
        }
    }

    /// <summary>
    /// 신호 연결을 해제합니다.
    /// </summary>
    public void DisconnectSignal<T>(string signalKey, Action<T> action)
    {
        if (_signals.TryGetValue(signalKey, out var existingDelegate))
        {
            var updatedDelegate = Delegate.Remove(existingDelegate, action);

            if (updatedDelegate == null)
                _signals.Remove(signalKey);
            else
                _signals[signalKey] = updatedDelegate;
        }
    }

    /// <summary>
    /// 신호를 호출합니다.
    /// </summary>
    public void EmitSignal<T>(string signalKey, T arg)
    {
        if (_signals.TryGetValue(signalKey, out var signal))
        {
            if (signal is Action<T> action)
            {
                action.Invoke(arg);
            }
            else
            {
                Debug.LogError(
                    $"SignalManager: Signal '{signalKey}' expects a different type. Provided: {typeof(T)}, Expected: {signal.GetType().GenericTypeArguments[0]}"
                );
            }
        }
        else
        {
            Debug.LogWarning($"SignalManager: Signal '{signalKey}' not found.");
        }
    }

    /// <summary>
    /// 디버그 정보를 출력합니다.
    /// 연결된 신호, 리스너 수, 연결된 타입 정보를 포함합니다.
    /// </summary>
    public void DebugSignals()
    {
        Debug.Log("===== SignalManager Debug Info =====");
        if (_signals.Count == 0)
        {
            Debug.Log("No signals are currently registered.");
            return;
        }

        foreach (var signal in _signals)
        {
            var invocationList = signal.Value?.GetInvocationList();
            var listenerCount = invocationList?.Length ?? 0;
            var type = signal.Value?.GetType().GenericTypeArguments[0] ?? typeof(void);

            Debug.Log($"Signal Key: {signal.Key}, Listener Count: {listenerCount}, Type: {type}");
        }

        Debug.Log("===================================");
    }
}