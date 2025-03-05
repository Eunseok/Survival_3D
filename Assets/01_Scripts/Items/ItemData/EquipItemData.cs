// Equip/ItemDataEquip.cs
using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New UEquipItem", menuName = "Items/Equip")]
    public class EquipItemData : ItemData
    {
        public bool isEquipped;
        public override ItemType Type => ItemType.Equip;
    }
}