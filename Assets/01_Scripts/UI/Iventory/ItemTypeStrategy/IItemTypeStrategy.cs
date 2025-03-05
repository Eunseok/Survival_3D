using Scripts.Items;
using TMPro;
using UnityEngine.UI;

public interface IItemTypeStrategy
{
    string GetButtonText(ItemSlot selectedItem);
    void ConfigureButtonAction(Button useButton, ItemSlot selectedItem);
    void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item);
}