using UnityEngine;

namespace Framework.Utilities
{
    /// <summary>
    /// 제네릭 형식 간에 정적 멤버를 공유하기 위해 비제네릭 기본 클래스를 정의합니다.
    /// </summary>
    public abstract class SingletonBase : MonoBehaviour
    {
        protected static readonly object LockObject = new object();
        protected static bool IsApplicationQuitting;
    }

    /// <summary>
    /// MonoBehaviour를 상속받은 싱글톤 베이스 제네릭 클래스입니다.
    /// </summary>
    /// <typeparam name="T">자식 클래스 타입</typeparam>
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

        /// <summary>
        /// Singleton 오브젝트를 생성합니다.
        /// </summary>
        private static T CreateSingletonInstance()
        {
            const string singletonObjectName = nameof(T);
            var singletonObject = new GameObject(singletonObjectName);
            var instance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);
            return instance;
        }

        /// <summary>
        /// 인스턴스 생명주기에서 재등록을 방지합니다.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }

            InitializeManager();
        }

        protected abstract void InitializeManager();

        /// <summary>
        /// 애플리케이션 종료 시 플래그 설정.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            IsApplicationQuitting = true;
        }
    }
}