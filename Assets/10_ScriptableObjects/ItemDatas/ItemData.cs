// Common/BaseItemData.cs

using UnityEngine;

namespace Scripts.Items
{
    public enum ItemType
    {
        Etc,
        Equip,
        Use
    }

    public interface IStackable
    {
        int MaxStackAmount { get; }
    }

    
    public abstract class ItemData : ScriptableObject
    {
        
        protected const int DefaultMaxStackAmount = 100;
        
        [Header("Info")]
        [SerializeField] private string itemName;
        public string ItemName => itemName;
        
        [SerializeField] private string itemDescription;
        public string ItemDescription => itemDescription;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] private GameObject dropPrefab;
        public GameObject DropPrefab => dropPrefab;
        
        public abstract ItemType Type { get; }
    }

}




