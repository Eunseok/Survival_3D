using UnityEngine;

public abstract class SingletonBase : MonoBehaviour
{
    protected static bool IsApplicationQuitting;
}

public class Singleton<T> : SingletonBase where T : MonoBehaviour
{
    protected static T instance;
    protected static bool IsDontDestroyOnLoad => true;

    public static T Instance
    {
        get
        {
            if(IsApplicationQuitting)
                return null;
            
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    var singletonObject = new GameObject(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                    if (IsDontDestroyOnLoad)
                      DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }
    
    private void OnApplicationQuit()
    {
        IsApplicationQuitting = true;
    }
}