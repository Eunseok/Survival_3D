// EquipItemData에 대한 전략 구현

using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemStrategy : IItemTypeStrategy
{
    public string GetButtonText(ItemSlot selectedItem)
    {
        return selectedItem.IsEquipped ? "장착해제" : "장착";
    }

    public void ConfigureButtonAction(Button useButton, ItemSlot selectedItem)
    {
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
            Debug.Log($"Toggling equipment: {selectedItem.item.ItemName}");
        });
    }

    public void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item)
    {
        statName.text = "장비 스탯 정보 없음";
        statValue.text = "";
    }
}