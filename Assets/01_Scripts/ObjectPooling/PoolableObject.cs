using UnityEngine;

namespace Framework.ObjectPooling
{
    /// <summary>
    /// 풀링 가능한 오브젝트의 기본 동작 정의.
    /// </summary>
    public abstract class PoolableObject : MonoBehaviour
    {
        /// <summary>
        /// 오브젝트가 재사용될 때 호출됩니다 (필요 시 Override).
        /// 기본 동작 정의가 필요하다면 구현.
        /// </summary>
        public virtual void OnObjectSpawn()
        {
        }

        /// <summary>
        /// 오브젝트가 반환될 때 호출됩니다 (필요 시 Override).
        /// 기본 동작 정의가 필요하다면 구현.
        /// </summary>
        public virtual void OnObjectReturn()
        {
        }
    }
}