using System;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager : Singleton<SignalManager>
{
    // 신호와 델리게이트를 관리하는 딕셔너리
    private readonly Dictionary<string, Delegate> _signals = new();

    /// <summary>
    /// 반환값이 없는 Action 신호를 연결합니다.
    /// </summary>
    public void ConnectSignal(string signalKey, Action action)
    {
        if (!_signals.TryAdd(signalKey, action))
        {
            // 기존 신호에 연결
            _signals[signalKey] = Delegate.Combine(_signals[signalKey], action);
        }
    }

    /// <summary>
    /// 반환값이 없는 Action(T) 신호를 연결합니다.(제네릭)
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
    /// 반환값이 있는 Func(T, TResult) 신호를 연결합니다.
    /// </summary>
    public void ConnectSignal<T, TResult>(string signalKey, Func<T, TResult> func)
    {
        if (!_signals.TryAdd(signalKey, func))
        {
            _signals[signalKey] = Delegate.Combine(_signals[signalKey], func);
        }
    }

    /// <summary>
    /// 반환값이 없는 Action 신호를 해제합니다.
    /// </summary>
    public void DisconnectSignal(string signalKey, Action action)
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
    /// 반환값이 없는 Action(T) 신호를 해제합니다.(제네릭)
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
    /// 반환값이 있는 Func(T, TResult) 신호를 해제합니다.
    /// </summary>
    public void DisconnectSignal<T, TResult>(string signalKey, Func<T, TResult> func)
    {
        if (_signals.TryGetValue(signalKey, out var existingDelegate))
        {
            var updatedDelegate = Delegate.Remove(existingDelegate, func);

            if (updatedDelegate == null)
                _signals.Remove(signalKey);
            else
                _signals[signalKey] = updatedDelegate;
        }
    }

    /// <summary>
    /// 반환값이 없는 Action 신호를 호출합니다.
    /// </summary>
    public void EmitSignal(string signalKey)
    {
        if (!_signals.TryGetValue(signalKey, out var signal))
        {
            Debug.LogWarning($"SignalManager: Signal '{signalKey}' not found.");
            return;
        }

        if (signal is Action action)
        {
            action.Invoke();
        }
        else
        {
            Debug.LogError($"SignalManager: Signal '{signalKey}' has an incompatible delegate type. Expected: Action.");
        }
    }

    /// <summary>
    /// 반환값이 없는 Action(T) 신호를 호출합니다.(제네릭)
    /// </summary>
    public void EmitSignal<T>(string signalKey, T arg)
    {
        if (!_signals.TryGetValue(signalKey, out var signal))
        {
            Debug.LogWarning($"SignalManager: Signal '{signalKey}' not found.");
            return;
        }

        if (signal is Action<T> action)
        {
            action.Invoke(arg);
        }
        else
        {
            Debug.LogError($"SignalManager: Signal '{signalKey}' expects a different type. Provided: {typeof(T)}");
        }
    }

    /// <summary>
    /// 반환값이 있는 Func(T, TResult) 신호를 호출합니다.
    /// </summary>
    public TResult EmitSignal<T, TResult>(string signalKey, T arg)
    {
        if (!_signals.TryGetValue(signalKey, out var signal))
        {
            Debug.LogWarning($"SignalManager: Signal '{signalKey}' not found.");
            return default;
        }

        if (signal is Func<T, TResult> func)
        {
            return func.Invoke(arg);
        }

        Debug.LogError($"SignalManager: Signal '{signalKey}' expects a different type. " +
                       $"Provided: Func<{typeof(T)}, {typeof(TResult)}>, " +
                       $"Expected: {signal.GetType()}");
        return default;
    }

    /// <summary>
    /// 연결된 모든 신호를 디버그 출력합니다.
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
            var type = signal.GetType();

            Debug.Log($"Signal Key: {signal.Key}, Listener Count: {listenerCount}, Type: {type}");
        }

        Debug.Log("===================================");
    }
}