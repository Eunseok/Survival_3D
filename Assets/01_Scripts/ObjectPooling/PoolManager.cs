using System.Collections.Generic;
using Framework.Utilities;
using UnityEngine;

namespace Framework.ObjectPooling
{
    /// <summary>
    /// 오브젝트 풀을 관리하는 PoolManager
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private readonly Dictionary<string, ObjectPool<PoolableObject>> _poolDictionary = new();

        /// <summary>
        /// Singleton 초기화 메서드
        /// </summary>
        protected override void InitializeManager()
        {
            // 필요한 초기화 코드 작성 가능
            Debug.Log("PoolManager Initialized");
        }

        /// <summary>
        /// 풀 생성
        /// </summary>
        /// <typeparam name="T">PoolableObject를 상속받은 타입</typeparam>
        /// <param name="key">풀 키</param>
        /// <param name="prefab">생성할 프리팹</param>
        /// <param name="initialSize">초기 풀 크기</param>
        /// <param name="maxSize">최대 풀 크기 (0 == 제한 없음)</param>
        public void CreatePool<T>(string key, T prefab, int initialSize, int maxSize = 0) where T : PoolableObject
        {
            if (_poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning($"Pool with key '{key}' already exists.");
                return;
            }

            var pool = new ObjectPool<T>(prefab, initialSize, transform, maxSize);
            _poolDictionary[key] = pool as ObjectPool<PoolableObject>;
        }

        /// <summary>
        /// 풀에서 오브젝트 가져오기
        /// </summary>
        /// <typeparam name="T">PoolableObject를 상속받은 타입</typeparam>
        /// <param name="key">풀 키</param>
        /// <param name="position">스폰 위치</param>
        /// <param name="rotation">스폰 회전</param>
        /// <returns>풀링된 오브젝트</returns>
        public T Spawn<T>(string key, Vector3 position, Quaternion rotation) where T : PoolableObject
        {
            if (_poolDictionary.TryGetValue(key, out var pool))
            {
                return pool.GetObject(position, rotation) as T;
            }

            Debug.LogError($"[PoolManager] Pool with key '{key}' not found!");
            return null;
        }

        /// <summary>
        /// 오브젝트 반환하기
        /// </summary>
        /// <typeparam name="T">PoolableObject를 상속받은 타입</typeparam>
        /// <param name="key">풀 키</param>
        /// <param name="obj">반환할 오브젝트</param>
        public void ReturnToPool<T>(string key, T obj) where T : PoolableObject
        {
            if (_poolDictionary.TryGetValue(key, out var pool))
            {
                pool.ReturnObject(obj);
                return;
            }

            Debug.LogError($"[PoolManager] Pool with key '{key}' not found!");
        }

        /// <summary>
        /// 등록된 풀 상태 디버그 출력
        /// </summary>
        public void PrintPoolDebug()
        {
            foreach (var pair in _poolDictionary)
            {
                Debug.Log($"Pool Key: {pair.Key}, Type: {pair.Value.GetType()}");
            }
        }
    }
}