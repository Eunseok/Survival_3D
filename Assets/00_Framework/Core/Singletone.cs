using UnityEngine;

public abstract class SingletonBase : MonoBehaviour
{
    protected static bool IsApplicationQuitting;
    protected static readonly bool IsDontDestroyOnLoad = true;
}

public class Singleton<T> : SingletonBase where T : MonoBehaviour
{
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            if(IsApplicationQuitting)
                return null;
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    var singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    if (IsDontDestroyOnLoad)
                      DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    private void OnApplicationQuit()
    {
        IsApplicationQuitting = true;
    }
}