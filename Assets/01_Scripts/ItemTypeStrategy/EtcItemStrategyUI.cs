// Null 전략: 처리 불가능한 아이템

using _01_Scripts.Gameplay.Entity.Characters.Player.Old;
using Scripts.Items;
using TMPro;
using UnityEngine.UI;

public class EtcItemStrategyUI : IItemTypeStrategyUI
{
    public string GetButtonText(ItemSlotData slotData) => string.Empty;

    public void ConfigureButtonAction(Button useButton, ItemSlotData slotData)
    {
    }

    public void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item)
    {
        statName.text = string.Empty;
        statValue.text = string.Empty;
    }
}