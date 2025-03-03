using System;
using Framework.UI.Scripts;
using UnityEngine;

namespace _01_Scripts.UI.Condition
{
    public class UICondition : BaseUI
    {
        [SerializeField]private UIConditionBar healthBar;
        [SerializeField]private UIConditionBar hungerBar;
        [SerializeField]private UIConditionBar staminaBar;


        private void Start()
        {
            StatHandler playerStatHandler = CharacterManager.Instance.Player.statHandler;
            playerStatHandler.Health.OnValueChangedUnityEvent += healthBar.SetCondition;
            playerStatHandler.Hunger.OnValueChangedUnityEvent += hungerBar.SetCondition;
            playerStatHandler.Stamina.OnValueChangedUnityEvent += staminaBar.SetCondition;
        }
    }
}