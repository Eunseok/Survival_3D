using UnityEngine;

namespace Framework.Utilities
{
    public abstract class SingletonBase : MonoBehaviour
    {
        protected static readonly object LockObject = new object();
        protected static bool IsApplicationQuitting;
        protected static bool IsDontDestroyOnLoad = true;
        
        /// <summary>
        /// DontDestroyOnLoad 활성화 여부를 설정할 수 있는 플래그입니다.
        /// 기본값은 true입니다.
        /// </summary>
        protected void SetDontDestroyOnLoad(bool value)
        {
            IsDontDestroyOnLoad = value;
        }
    }

    public abstract class Singleton<T> : SingletonBase where T : MonoBehaviour
    {
        private static T _instance;

        /// <summary>
        /// 싱글톤 인스턴스에 대한 전역 접근자입니다.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (IsApplicationQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }

                lock (LockObject)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>() ?? CreateSingletonInstance();
                    }
                }

                return _instance;
            }
        }
        

        private static T CreateSingletonInstance()
        {
            const string singletonObjectName = nameof(T);
            var singletonObject = new GameObject(singletonObjectName);
            var instance = singletonObject.AddComponent<T>();

            if (IsDontDestroyOnLoad)
            {
                DontDestroyOnLoad(singletonObject);
            }

            return instance;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (IsDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }

            InitializeManager();
        }

        protected abstract void InitializeManager();

        protected virtual void OnApplicationQuit()
        {
            IsApplicationQuitting = true;
        }
    }
}