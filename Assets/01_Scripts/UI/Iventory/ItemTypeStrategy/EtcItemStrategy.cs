// Null 전략: 처리 불가능한 아이템

using Scripts.Items;
using TMPro;
using UnityEngine.UI;

public class EtcItemStrategy : IItemTypeStrategy
{
    public string GetButtonText(ItemSlot selectedItem) => string.Empty;

    public void ConfigureButtonAction(Button useButton, ItemSlot selectedItem)
    {
    }

    public void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item)
    {
        statName.text = string.Empty;
        statValue.text = string.Empty;
    }
}