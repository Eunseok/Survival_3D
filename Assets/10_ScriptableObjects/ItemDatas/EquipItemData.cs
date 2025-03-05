// Equip/ItemDataEquip.cs

using System;
using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New UEquipItem", menuName = "Items/Equip")]
    public class EquipItemData : ItemData
    {
        //public Action OnEquip;

        public bool isEquipped;

        public bool IsEquipped
        {
            get => isEquipped;
            set
            {
                isEquipped = value;
                //OnEquip?.Invoke();
            }
        }
        public override ItemType Type => ItemType.Equip;
        
        [Header("Equip")]
        public GameObject equipPrefab;
    }
}