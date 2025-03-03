using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts
{
    public class CampFire : MonoBehaviour
    {
        [SerializeField] private float damagePerTick;
        [SerializeField] private float tickInterval;

        private readonly HashSet<IDamageable> objectsInFire = new();
        private readonly Dictionary<IDamageable, Coroutine> activeCoroutines = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                if (!objectsInFire.Contains(damageable))
                {
                    objectsInFire.Add(damageable);
                    activeCoroutines[damageable] = StartCoroutine(ApplyBurnDamage(damageable));
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                if (objectsInFire.Contains(damageable))
                {
                    objectsInFire.Remove(damageable);

                    // 해당 코루틴 정지
                    if (activeCoroutines.ContainsKey(damageable))
                    {
                        StopCoroutine(activeCoroutines[damageable]);
                        activeCoroutines.Remove(damageable);
                    }
                }
            }
        }

        private IEnumerator ApplyBurnDamage(IDamageable target)
        {
            // 첫 번째 데미지는 즉시 적용
            target.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);

            // 이후 tickInterval마다 데미지를 적용
            while (objectsInFire.Contains(target))
            {
                target.TakeDamage(damagePerTick);
                yield return new WaitForSeconds(tickInterval);
            }
        }
    }
}