using System;
using System.Collections.Generic;
using Managers;
using Scripts.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts.Gameplay.Entity.Characters.Player.Old
{
    public class InventorySystem : MonoBehaviour
    {
        public List<ItemSlotData> Slots { get; private set; }

        [SerializeField] private int slotCount = 14;
        public int SlotCount => Slots.Count;
        public Transform dropPoint;
        private PopupInventory _popupInventory;

        private void Awake()
        {
            GetComponent<global::Player>().AddItem += AddItem;
        }

        private void OnEnable()
        {
            InputManager.Instance.OnInventoryPressed += OnInventoryPressed;
        }

        private void OnDisable()
        {
            if (InputManager.Instance)
                InputManager.Instance.OnInventoryPressed -= OnInventoryPressed;
        }

        private void OnInventoryPressed()
        {
            if (_popupInventory == null)
            {
                _popupInventory = UIManager.Instance.ShowPopup<PopupInventory>();
                _popupInventory.Initialize(Slots, dropPoint);
            }
            else
            {
                UIManager.Instance.ClosePopup(_popupInventory);
            }
        }

        private void Start()
        {
            InitializeInventory();
        }

        private void InitializeInventory()
        {
            Slots = new List<ItemSlotData>();
            for (int i = 0; i < slotCount; i++)
            {
                var itemSlotData = new ItemSlotData();
                Slots.Add(itemSlotData);
            }
        }

        private void AddItem(ItemData itemData)
        {
            // 스택 가능한 아이템이라면
            if (itemData is IStackable stackable)
            {
                var slot = GetItemStack(stackable);
                if (slot != null)
                {
                    slot.Quantity++;
                    return;
                }
            }

            // 빈 슬롯 찾기
            var emptySlot = GetEmptySlot();
            if (emptySlot != null)
            {
                emptySlot.Item = itemData;
                emptySlot.Quantity = 1; // 기본 수량 설정
                return;
            }

            // 빈 슬롯 마저 없을 때
            ThrowItem(itemData);
        }

        /// 빈 슬롯을 찾음
        private ItemSlotData GetEmptySlot()
        {
            foreach (var slot in Slots)
            {
                if (slot.Item == null)
                    return slot;
            }

            return null;
        }

        /// 아이템 삭제 (버림)
        private void ThrowItem(ItemData data)
        {
            if (data == null) return;
            Instantiate(data.DropPrefab, dropPoint.position, Quaternion.Euler(Vector3.one * Random.value * 360));
        }

        // 스택 가능한 아이템 슬롯을 찾음
        private ItemSlotData GetItemStack(IStackable data)
        {
            foreach (var slot in Slots)
            {
                if (slot.Item == (ItemData)data && slot.Quantity < data.MaxStackAmount)
                    return slot;
            }

            return null;
        }

        public void RemoveItem(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < Slots.Count)
            {
                Slots[slotIndex].Clear();
            }
        }
    }

// 슬롯 데이터를 독립적으로 관리하는 클래스
    public class ItemSlotData
    {
        public Action OnItemChanged = delegate { };

        private bool _isEquipped;

        public bool IsEquipped
        {
            get => _isEquipped;
            set
            {
                if (_isEquipped == value) return;
                _isEquipped = value;
                OnItemChanged?.Invoke();
            }
        }

        private ItemData _item;

        public ItemData Item
        {
            get => _item;
            set
            {
                if (Item == value) return;
                _item = value;
                OnItemChanged?.Invoke();
            }
        }

        private int _quantity;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (Quantity == value) return;
                _quantity = value;
                OnItemChanged?.Invoke();
                Debug.Log($"OnItemChanged 상태: {(OnItemChanged == null ? "null" : "구독자 있음")}");
            }
        }

        public void Clear()
        {
            Item = null;
            Quantity = 0;
            IsEquipped = false;
        }

        public bool RemoveSelectedItem()
        {
            Quantity--;

            if (Quantity > 0) return false;
            if (Item is EquipItemData && IsEquipped)
            {
                // UnEquip(selectedItemIndex);
            }

            Clear();
            return true;
        }
    }
}