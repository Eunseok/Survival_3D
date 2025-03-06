using Scripts.UI;
using UnityEngine;

namespace _01_Scripts.UI
{
    public class HUDGame : UIHud
    {
        public enum HudType
        {
            Conditions,
            DamageIndicator,
            PromptText
        }

        protected override void Awake()
        {
            base.Awake();
            AutoBind<GameObject>(typeof(HudType));
        }
    }
}