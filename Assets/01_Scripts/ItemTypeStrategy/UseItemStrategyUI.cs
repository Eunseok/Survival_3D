using System.Globalization;
using Scripts.Items;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using _01_Scripts.Gameplay.Entity.Characters.Player.Old;
using Managers;

// UseItemData에 대한 전략 구현
public class UseItemStrategyUI : IItemTypeStrategyUI
{
    public string GetButtonText(ItemSlotData slotData)
    {
        return "사용";
    }

    public void ConfigureButtonAction(Button useButton, ItemSlotData slotData)
    {
        useButton.onClick.AddListener(() =>
        {
            var useItemData = slotData.Item as UseItemData;
            if (useItemData != null)
                foreach (var effect in useItemData.Consumables)
                {
                    switch (effect.type)
                    {
                        case ConsumableType.Health:
                            SignalManager.Instance.EmitSignal("OnPlayerHeal", effect.value);
                            break;
                        case ConsumableType.Hunger:
                            SignalManager.Instance.EmitSignal("OnPlayerEat", effect.value);
                            break;
                        case ConsumableType.Speed:
                            SignalManager.Instance.EmitSignal("OnApplySpeedBuff", effect.value, effect.duration);
                            break;
                    }
                }

            slotData.RemoveSelectedItem();
        });
    }

    public void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item)
    {
        if (item is UseItemData useItem)
        {
            statName.text = string.Join("\n", useItem.Consumables.Select(c => c.type.ToString()));
            statValue.text = string.Join("\n", useItem.Consumables.Select(c => c.value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}