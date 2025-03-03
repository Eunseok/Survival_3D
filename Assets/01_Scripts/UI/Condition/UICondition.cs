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
            playerStatHandler.Health.onValueChangedUnityEvent += healthBar.SetCondition;
            playerStatHandler.Hunger.onValueChangedUnityEvent += hungerBar.SetCondition;
            playerStatHandler.Stamina.onValueChangedUnityEvent += staminaBar.SetCondition;
        }
    }
}