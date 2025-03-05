using System.Collections.Generic;
using Scripts.Characters;
using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIInventory : UIBase
{
    [Header("UI References")] public List<ItemSlot> slots; // 인벤토리에 포함된 모든 슬롯 리스트
    public GameObject slotPrefab; // 슬롯 프리팹
    public Transform dropPosition;

    private int _selectedIndex = -1; // 현재 선택된 슬롯 인덱스 (-1은 선택되지 않음)
    private readonly int _slotCount = 14; // 기본 슬롯 개수

    enum GameObjects
    {
        SlotContainer,
        InventoryWindow
    }

    enum Texts
    {
        SelectedItemName,
        SelectedItemDescription,
        SelectedItemStatName,
        SelectedItemStatValue,
        UseButton,
        //DropButton
    }

    enum Buttons
    {
        UseButton,
        DropButton
    }

    private void Start()
    {
        AutoBind<GameObject>(typeof(GameObjects));
        AutoBind<TextMeshProUGUI>(typeof(Texts));
        AutoBind<Button>(typeof(Buttons));

        InitializeInventory(_slotCount);
        InputManager.Instance.OnInventoryPressed += Toggle;
        CharacterManager.Instance.Player.AddItem += AddItem;
        GetObject((int)GameObjects.InventoryWindow).SetActive(false);
    }

    private void Toggle()
    {
        GetObject((int)GameObjects.InventoryWindow).SetActive(!IsOpen());
    }

    private bool IsOpen()
    {
        return GetObject((int)GameObjects.InventoryWindow).activeInHierarchy;
    }

    /// <summary>
    /// 초기화 시 슬롯 생성 및 세팅
    /// </summary>
    private void InitializeInventory(int slotCount)
    {
        // 기존 슬롯 정리
        foreach (Transform child in GetObject((int)GameObjects.SlotContainer).transform)
            Destroy(child.gameObject);

        slots.Clear();

        // 새로운 슬롯 생성 및 추가
        for (int i = 0; i < slotCount; i++)
        {
            var slotGo = Instantiate(slotPrefab, GetObject((int)GameObjects.SlotContainer).transform);
            var itemSlot = slotGo.GetComponent<ItemSlot>();
            itemSlot.Set(null, 0, this, i); // 아이템 없는 빈 슬롯으로 초기화
            slots.Add(itemSlot);
        }

        ClearSelectedItemWindow();
    }

    /// <summary>
    /// 특정 슬롯에 아이템 추가 또는 수정
    /// </summary>
    public void AddOrUpdateItem(int slotIndex, ItemData newItem, float quantity)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
            slots[slotIndex].Set(newItem, quantity, this, slotIndex);
    }

    private void AddItem(ItemData data)
    {
        // 스택 가능한 아이템이라면
        if (data is IStackable stackable)
        {
            var slot = GetItemStack(stackable);
            if (slot != null)
            {
                slot.AddStackItem();
                return;
            }
        }

        // 빈 슬롯 찾기
        var emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.AddNewItem(data, 1);
            return;
        }

        // 빈 슬롯 마저 없을 때
        ThrowItem(data);
    }

    /// <summary>
    /// 스택 가능한 아이템 슬롯을 찾음
    /// </summary>
    private ItemSlot GetItemStack(IStackable data)
    {
        foreach (var slot in slots)
        {
            if (slot.item == (ItemData)data && slot.CanStack)
                return slot;
        }

        return null;
    }

    /// <summary>
    /// 빈 슬롯을 찾음
    /// </summary>
    private ItemSlot GetEmptySlot()
    {
        foreach (var slot in slots)
        {
            if (slot.item == null)
                return slot;
        }

        return null;
    }

    /// <summary>
    /// 아이템 삭제 (버림)
    /// </summary>
    private void ThrowItem(ItemData data)
    {
        if (data == null) return;
        Instantiate(data.DropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    /// <summary>
    /// 특정 슬롯의 아이템 제거
    /// </summary>
    public void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
            slots[slotIndex].ClearUI();
    }

    /// <summary>
    /// 특정 슬롯을 선택
    /// </summary>
    public void SelectItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            // 이전 선택된 슬롯 비활성화
            if (_selectedIndex >= 0 && _selectedIndex < slots.Count)
                slots[_selectedIndex].UpdateEquippedStatus(false);

            // 새로운 슬롯 선택
            _selectedIndex = slotIndex;
            slots[slotIndex].UpdateEquippedStatus(true);
            UpdateSelectedItemUI(slots[slotIndex]); // 선택된 아이템 관련 UI 업데이트
        }
    }

    /// <summary>
    /// 선택된 아이템 관련 UI 업데이트
    /// </summary>
    public void UpdateSelectedItemUI(ItemSlot selectedItem)
    {
        if (!selectedItem.item)
        {
            ClearSelectedItemWindow();
            return;
        }

        var strategy = GetStrategy(selectedItem.item); // 전략 패턴 가져오기

        // 선택된 아이템의 정보를 UI에 반영

        GetText((int)Texts.SelectedItemName).text = selectedItem.item.ItemName;
        GetText((int)Texts.SelectedItemDescription).text = selectedItem.item.ItemDescription;
        strategy.UpdateStats(GetText((int)Texts.SelectedItemStatName)
            , GetText((int)Texts.SelectedItemStatValue), selectedItem.item);

        // 버튼 관련 UI 업데이트
        UpdateButtons(strategy, selectedItem);
    }

    private void UpdateButtons(IItemTypeStrategy strategy, ItemSlot selectedItem)
    {
        ConfigureUseButton(strategy, selectedItem);
        ConfigureDropButton();
    }

    private void ConfigureUseButton(IItemTypeStrategy strategy, ItemSlot selectedItem)
    {
        Button useButton = GetButton((int)Buttons.UseButton);

        string buttonText = strategy.GetButtonText(selectedItem);
        UpdateButtonUI(useButton, buttonText);

        useButton.onClick.RemoveAllListeners();
        strategy.ConfigureButtonAction(useButton, selectedItem);
        useButton.onClick.AddListener(RemoveSelectedItem);
    }

    private void ConfigureDropButton()
    {
        Button dropButton = GetButton((int)Buttons.DropButton);

        dropButton.gameObject.SetActive(true); // 기본 활성화

        dropButton.onClick.RemoveAllListeners();
        dropButton.onClick.AddListener(OnDropButton); // "버리기" 버튼 설정
    }

    private void UpdateButtonUI(Button button, string text)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        button.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }

    private void ClearSelectedItemWindow()
    {
        GetText((int)Texts.SelectedItemName).text = string.Empty;
        GetText((int)Texts.SelectedItemDescription).text = string.Empty;
        GetText((int)Texts.SelectedItemStatName).text = string.Empty;
        GetText((int)Texts.SelectedItemStatValue).text = string.Empty;

        GetButton((int)Buttons.UseButton).gameObject.SetActive(false);
        GetButton((int)Buttons.DropButton).gameObject.SetActive(false);
    }

    private void RemoveSelectedItem()
    {
        slots[_selectedIndex].RemoveSelectedItem();
    }

    private IItemTypeStrategy GetStrategy(ItemData item)
    {
        return item switch
        {
            UseItemData => new UseItemStrategy(),
            EquipItemData => new EquipItemStrategy(),
            _ => new EtcItemStrategy()
        };
    }

    /// <summary>
    /// "버리기" 버튼 클릭 이벤트
    /// </summary>
    private void OnDropButton()
    {
        ThrowItem(slots[_selectedIndex].item);
        RemoveSelectedItem();
    }

    /// <summary>
    /// 모든 슬롯 초기화
    /// </summary>
    public void ClearAllSlots()
    {
        foreach (var slot in slots)
            slot.ClearUI();
    }
}