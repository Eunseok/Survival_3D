using Scripts.Items;
using TMPro;
using UnityEngine.UI;

public interface IItemTypeStrategyUI
{
    string GetButtonText(ItemSlotData slotData);
    void ConfigureButtonAction(Button useButton, ItemSlotData slotData);
    void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item);
}