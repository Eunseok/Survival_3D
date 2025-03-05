using Scripts.Items;
using TMPro;
using UnityEngine.UI;

public interface IItemTypeStrategyUI
{
    string GetButtonText(ItemData selectedItem);
    void ConfigureButtonAction(Button useButton, ItemData selectedItem);
    void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item);
}