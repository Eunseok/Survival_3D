using System.Collections.Generic;
using UnityEngine;

namespace Framework.ObjectPooling
{
    /// <summary>
    /// 제네릭 오브젝트 풀링 시스템
    /// </summary>
    public class ObjectPool<T> where T : PoolableObject
    {
        private readonly Queue<T> _poolQueue = new();
        private readonly T _prefab;
        private readonly Transform _parentTransform;
        private readonly int _maxPoolSize; // 최대 풀 크기 (0이면 제한 없음)
        private int _currentCount;

        /// <summary>
        /// 풀 생성자
        /// </summary>
        /// <param name="prefab">풀링할 프리팹</param>
        /// <param name="initialSize">초기 생성 크기</param>
        /// <param name="parent">오브젝트를 보관할 부모 Transform</param>
        /// <param name="maxPoolSize">최대 풀 크기</param>
        public ObjectPool(T prefab, int initialSize, Transform parent = null, int maxPoolSize = 0)
        {
            _prefab = prefab;
            _parentTransform = parent;
            _maxPoolSize = maxPoolSize;

            // 초기 오브젝트 생성
            for (int i = 0; i < initialSize; i++)
            {
                AddNewObjectToPool();
            }
        }

        /// <summary>
        /// 새로운 오브젝트를 생성하고 풀에 추가합니다.
        /// </summary>
        private void AddNewObjectToPool()
        {
            T newObj = Object.Instantiate(_prefab, _parentTransform);
            newObj.gameObject.SetActive(false);
            _poolQueue.Enqueue(newObj);
            _currentCount++;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.
        /// </summary>
        /// <param name="position">스폰 위치</param>
        /// <param name="rotation">스폰 회전값</param>
        /// <returns>풀에서 가져온 활성화된 오브젝트</returns>
        public T GetObject(Vector3 position, Quaternion rotation)
        {
            T obj;

            if (_poolQueue.Count > 0)
            {
                obj = _poolQueue.Dequeue();
            }
            else
            {
                if (_maxPoolSize == 0 || _currentCount < _maxPoolSize)
                {
                    AddNewObjectToPool();
                    obj = _poolQueue.Dequeue();
                }
                else
                {
                    Debug.LogError($"Pool exceeded max size: {_maxPoolSize}");
                    return null;
                }
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);
            obj.OnObjectSpawn(); // PoolableObject의 스폰 초기화 메서드 호출

            return obj;
        }

        /// <summary>
        /// 오브젝트를 반환합니다.
        /// </summary>
        /// <param name="obj">반환할 오브젝트</param>
        public void ReturnObject(T obj)
        {
            if (_poolQueue.Contains(obj))
            {
                Debug.LogError("Object is already in the pool!");
                return;
            }

            obj.OnObjectReturn(); // PoolableObject의 반환 초기화 메서드 호출
            obj.gameObject.SetActive(false);
            _poolQueue.Enqueue(obj);
        }
    }
}