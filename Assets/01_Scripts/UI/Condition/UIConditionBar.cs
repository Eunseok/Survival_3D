using UnityEngine;
using UnityEngine.UI;

public class UIConditionBar : MonoBehaviour
{
    [SerializeField]private Image imgProgress;

    public void SetCondition(float curValue, float maxValue)
    {
        imgProgress.fillAmount = Mathf.Clamp01(curValue/ maxValue);
    }
}