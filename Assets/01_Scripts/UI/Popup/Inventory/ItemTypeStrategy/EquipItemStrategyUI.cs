// EquipItemData에 대한 전략 구현

using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemStrategyUI : IItemTypeStrategyUI
{
    public string GetButtonText(ItemData selectedItem)
    {
        var equipItem = selectedItem as EquipItemData; // selectedItem을 EquipItemData로 형변환
        if (equipItem == null)
            return "유효하지 않은 아이템";

        return equipItem.IsEquipped ? "장착해제" : "장착";

    }

    public void ConfigureButtonAction(Button useButton, ItemData selectedItem)
    {
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
            Debug.Log($"Toggling equipment: {selectedItem.ItemName}");
        });
    }

    public void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item)
    {
        statName.text = "장비 스탯 정보 없음";
        statValue.text = "";
    }
}