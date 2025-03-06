// Equip/ItemDataEquip.cs

using System;
using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New UEquipItem", menuName = "Items/Equip")]
    public class EquipItemData : ItemData
    {
        public override ItemType Type => ItemType.Equip;
        
        [Header("Equip")]
        public GameObject equipPrefab;
    }
}