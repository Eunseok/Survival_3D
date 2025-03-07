// Use/ItemDataUse.cs
using System;
using UnityEngine;

namespace Scripts.Items
{
    public enum ConsumableType
    {
        Hunger,
        Health,
        Speed,
    }
    
    [Serializable]
    public class ItemDataConsumable
    {
        public ConsumableType type;
        public float value;
        public float duration;
    }
    
    [CreateAssetMenu(fileName = "New UseItem", menuName = "Items/Use")]
    public class UseItemData : ItemData, IStackable
    {
        [Header("Consumable")]
        [SerializeField] private ItemDataConsumable[] consumables;
        public ItemDataConsumable[] Consumables => consumables;
        
        [Header("Stacking")] 
        [SerializeField] private int maxStackAmount = DefaultMaxStackAmount;
        public int MaxStackAmount => maxStackAmount;
        
        public override ItemType Type => ItemType.Use;
    }
}