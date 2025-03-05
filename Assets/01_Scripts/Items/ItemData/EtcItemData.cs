// Etc/ItemDataEtc.cs
using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(fileName = "New UEtcItem", menuName = "Items/Etc")]
    public class EtcItemData : ItemData, IStackable
    {
        [Header("Stacking")] 
        [SerializeField] private int maxStackAmount = DefaultMaxStackAmount;
        public int MaxStackAmount => maxStackAmount;

        public override ItemType Type => ItemType.Etc;
    }
}