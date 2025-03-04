using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Condition
{
    public class UIConditionBar : MonoBehaviour
    {
        [SerializeField]private Image imgProgress;

        public void SetCondition(float curValue, float maxValue)
        {
            // maxValue가 0일 경우 fillAmount를 0으로 설정하여 나눗셈을 방지
            if (maxValue == 0)
            {
                imgProgress.fillAmount = 0;
                return;
            }
            
            imgProgress.fillAmount = Mathf.Clamp01(curValue/ maxValue);
        }
    }
}