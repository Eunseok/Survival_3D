using Scripts.Items;
using TMPro;
using UnityEngine.UI;
using System.Linq;

// UseItemData에 대한 전략 구현
public class UseItemStrategyUI : IItemTypeStrategyUI
{
    public string GetButtonText(ItemData selectedItem)
    {
        return "사용";
    }

    public void ConfigureButtonAction(Button useButton, ItemData selectedItem)
    {
        useButton.onClick.AddListener(() =>
        {
            var useItemData = selectedItem as UseItemData;
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
                    }
                }
        });
    }

    public void UpdateStats(TextMeshProUGUI statName, TextMeshProUGUI statValue, ItemData item)
    {
        if (item is UseItemData useItem)
        {
            statName.text = string.Join("\n", useItem.Consumables.Select(c => c.type.ToString()));
            statValue.text = string.Join("\n", useItem.Consumables.Select(c => c.value.ToString()));
        }
    }
}