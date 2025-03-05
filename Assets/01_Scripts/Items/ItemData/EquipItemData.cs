// Equip/ItemDataEquip.cs
using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New UEquipItem", menuName = "Items/Equip")]
    public class EquipItemData : ItemData
    {
        private bool _isEquipped;
        public bool IsEquipped => _isEquipped;
        public override ItemType Type => ItemType.Equip;
    }
}