using UnityEngine;

public class SingletonBase:MonoBehaviour
{
    protected static bool IsQuitting; // 종료 상태 플래그
}

public class Singleton<T> : SingletonBase  where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // 이미 존재하는 인스턴스 반환
            if (_instance != null) return _instance;

         
            if (IsQuitting) return null;
                            
            // 활성화된 인스턴스 탐색
            _instance = FindObjectOfType<T>();
            if (_instance != null) return _instance;

            // 새 GameObject 동적 생성
            var singletonObject = new GameObject(typeof(T).Name);
            _instance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);

            Debug.Log($"{typeof(T).Name} instance dynamically created.");
            return _instance;
        }
    }
    
    public static bool HasInstance => _instance != null;
    
    // 애플리케이션 종료 시 호출
    protected virtual void OnApplicationQuit()
    {
        IsQuitting = true;
    }

}