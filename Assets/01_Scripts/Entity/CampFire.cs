using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts
{
    public class CampFire : MonoBehaviour
    {
        [SerializeField] private float damagePerTick;
        [SerializeField] private float tickInterval;

        private readonly HashSet<IDamageable> _objectsInFire = new();
        private readonly Dictionary<IDamageable, Coroutine> _activeCoroutines = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                if (_objectsInFire.Add(damageable))
                {
                    _activeCoroutines[damageable] = StartCoroutine(ApplyBurnDamage(damageable));
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                if (_objectsInFire.Contains(damageable))
                {
                    _objectsInFire.Remove(damageable);

                    // 해당 코루틴 정지
                    if (_activeCoroutines.ContainsKey(damageable))
                    {
                        StopCoroutine(_activeCoroutines[damageable]);
                        _activeCoroutines.Remove(damageable);
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
            while (_objectsInFire.Contains(target))
            {
                target.TakeDamage(damagePerTick);
                yield return new WaitForSeconds(tickInterval);
            }
        }
    }
}