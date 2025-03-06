// EquipItemData에 대한 전략 구현

using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemStrategyUI : IItemTypeStrategyUI
{
    public string GetButtonText(ItemSlotData slotData)
    {
        Debug.Log(slotData.IsEquipped);
        return slotData.IsEquipped ? "장착해제" : "장착";
    }

    public void ConfigureButtonAction(Button useButton, ItemSlotData slotData)
    {
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
                SignalManager.Instance.EmitSignal(!slotData.IsEquipped ? "OnPlayerEquip" : "OnPlayerUnEquip",
                    slotData);
        });
    }

    public void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item)
    {
        statName.text = "장비 스탯 정보 없음";
        statValue.text = "";
    }
}