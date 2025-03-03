using System;
using UnityEngine;
using UnityEngine.UI;

public class UIConditionBar : MonoBehaviour
{
    [SerializeField]private Image imgProgress;

    public void SetCondition(float curValue, float maxValue)
    {
        imgProgress.fillAmount = curValue/ maxValue;
    }
}