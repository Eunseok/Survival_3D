// Equip/ItemDataEquip.cs
using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New UEquipItem", menuName = "Items/Equip")]
    public class EquipItemData : ItemData
    {
        // [Header("Equip Stats")] [SerializeField]
        // private int durability;
        // public int Durability => durability;
        //
        public override ItemType Type => ItemType.Equip;
    }
}