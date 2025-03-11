using Scripts.Characters;
using UnityEngine;

namespace Scripts.UI.Condition
{
    public class UICondition : MonoBehaviour
    {
        [SerializeField]private UIConditionBar healthBar;
        [SerializeField]private UIConditionBar staminaBar;


        private void Start()
        {
            Player player = CharacterManager.Instance.Player;
            if (player != null)
            {
                StatHandler playerStatHandler = player.statHandler;
                playerStatHandler.health.OnValueChangedUnityEvent += healthBar.SetCondition;
                playerStatHandler.stamina.OnValueChangedUnityEvent += staminaBar.SetCondition;
            }
            else
            {
                Debug.Log("Player is null");
            }
        }
    }
}