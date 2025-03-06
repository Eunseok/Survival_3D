using Scripts.Characters;
using UnityEngine;

namespace Scripts.UI.Condition
{
    public class UICondition : MonoBehaviour
    {
        [SerializeField]private UIConditionBar healthBar;
        [SerializeField]private UIConditionBar hungerBar;
        [SerializeField]private UIConditionBar staminaBar;


        private void Start()
        {
            StatHandler playerStatHandler = CharacterManager.Instance.Player.statHandler;
            playerStatHandler.health.OnValueChangedUnityEvent += healthBar.SetCondition;
            playerStatHandler.hunger.OnValueChangedUnityEvent += hungerBar.SetCondition;
            playerStatHandler.stamina.OnValueChangedUnityEvent += staminaBar.SetCondition;
        }
    }
}