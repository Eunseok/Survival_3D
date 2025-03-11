using System;
using UnityEngine;
using UnityEngine.Events;

namespace _01_Scripts.Gameplay.Entity.Characters.Player.Old
{
    [Serializable]
    public class Condition
    {
        public event UnityAction<float, float> OnValueChangedUnityEvent;

        [SerializeField] private float maxValue;
        [SerializeField] private float curValue;
        public float CurValue=> curValue;
        [SerializeField] private float passiveValue;


        public void Apply(float amount)
        {
            curValue = Mathf.Clamp(curValue + amount, 0f, maxValue);

            OnValueChangedUnityEvent?.Invoke(curValue, maxValue);
        }


        public void PassiveApply()
        {
            Apply(passiveValue * Time.deltaTime);
        }
    }
}


