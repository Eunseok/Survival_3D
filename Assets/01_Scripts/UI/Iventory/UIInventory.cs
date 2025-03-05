using Scripts.Characters;
using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;
    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Selected Item")] 
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public Button useButton;
    public Button dropButton;

    private void Start()
    {
        InputManager.Instance.OnInventoryPressed += Toggle;
        CharacterManager.Instance.Player.AddItem += AddItem;
        
        // Inventory UI 초기화 로직들
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData data)
    {
        // 여러개 가질 수 있는 아이템이라면
        if (data is IStackable stackable)
        {
            ItemSlot slot = GetItemStack(stackable);
            if(slot != null)
            {
                slot.quantity++;
                UpdateSlots();
                return;
            }
        }
        
        // 빈 슬롯 찾기
        ItemSlot emptySlot = GetEmptySlot();
        
        // 빈 슬롯이 있다면
        if(emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateSlots();
            return;
        }
        
        // 빈 슬롯 마저 없을 때
         ThrowItem(data);
    }
    
    // 여러개 가질 수 있는 아이템의 정보 찾아서 return
    ItemSlot GetItemStack(IStackable data)
    {
        foreach (var slot in slots)
        {
            if (slot.item == (ItemData)data && slot.quantity < data.MaxStackAmount)
            {
                return slot;
            }
        }

        return null;
    }
    
    // 슬롯의 item 정보가 비어있는 정보 return
    ItemSlot GetEmptySlot()
    {
        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                return slot;
            }
        }

        return null;
    }
    
    // 아이템 버리기 (실제론 매개변수로 들어온 데이터에 해당하는 아이템 생성)
    public void ThrowItem(ItemData data)
    {
        if(data == null) return;
        Instantiate(data.DropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }
    
    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(false);
    }
    
    void RemoveSelctedItem()
    {
        selectedItem.quantity--;

        if(selectedItem.quantity <= 0)
        {
            if (slots[selectedItemIndex].equipped)
            {
               // UnEquip(selectedItemIndex);
            }

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }
    
    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        SetSelectedItem(index);
        UpdateUI();
    }

    private void SetSelectedItem(int index)
    {
        selectedItem = slots[index];
        selectedItemIndex = index;
    }

    private void UpdateUI()
    {
        UpdateSlots();     // 슬롯 관련 UI 업데이트
        UpdateSelectedItemUI(); // 선택된 아이템 관련 UI 업데이트
    }

    private void UpdateSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.item != null)
            {
                slot.Set(); // 슬롯에 아이템이 있다면 세팅
            }
            else
            {
                slot.Clear(); // 슬롯에 아이템이 없다면 초기화
            }
        }
    }

    private void UpdateSelectedItemUI()
    {
        if (!selectedItem) return;

        var item = selectedItem.item;

        // 전략 가져오기
        var strategy = GetStrategy(item);

        // 선택된 아이템의 정보를 UI에 반영
        selectedItemName.text = item.ItemName;
        selectedItemDescription.text = item.ItemDescription;
        strategy.UpdateStats(selectedItemStatName, selectedItemStatValue, item);

        // 버튼 관련 UI 업데이트
        UpdateButtons(strategy);
    }

    private void UpdateButtons(IItemTypeStrategy strategy)
    {
        ConfigureUseButton(strategy);
        ConfigureDropButton();
    }

    private void ConfigureUseButton(IItemTypeStrategy strategy)
    {
        string buttonText = strategy.GetButtonText(selectedItem);
        UpdateButtonUI(useButton, buttonText);
        
        useButton.onClick.RemoveAllListeners();
        strategy.ConfigureButtonAction(useButton, selectedItem);
        useButton.onClick.AddListener(RemoveSelctedItem);
    }

    private void ConfigureDropButton()
    {
        dropButton.gameObject.SetActive(true); // 기본 활성화
        
        dropButton.onClick.RemoveListener(OnDropButton);
        dropButton.onClick.AddListener(OnDropButton); // 버리기말고 다른버튼 생길까봐
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelctedItem();
    }
    
    private void UpdateButtonUI(Button button, string text)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        button.gameObject.SetActive(!string.IsNullOrEmpty(text));
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
}