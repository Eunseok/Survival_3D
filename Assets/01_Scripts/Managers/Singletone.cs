// Singleton.cs

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected static bool IsDontDestroyOnLoad => true;

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;

        if (IsDontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        Debug.Log($"{typeof(T).Name} Initialized");
    }
}