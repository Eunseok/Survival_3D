using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Condition
{
    public UnityAction<float, float> onValueChangedUnityEvent;

    [SerializeField] private float maxValue;
    [SerializeField] public float curValue; // Field로 변경
    [SerializeField] private float passiveValue;


    public void Apply(float amount)
    {
        curValue = Mathf.Clamp(curValue + amount, 0f, maxValue);

        onValueChangedUnityEvent.Invoke(curValue, maxValue);
    }


    public void PassiveApply()
    {
        curValue += passiveValue * Time.deltaTime;
        onValueChangedUnityEvent.Invoke(curValue, maxValue);
    }
}