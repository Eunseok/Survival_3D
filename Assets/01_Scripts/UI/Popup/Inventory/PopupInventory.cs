using System.Collections.Generic;
using Scripts.Items;
using Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PopupInventory : UIPopup
{
    [Header("UI References")]
    private readonly List<ItemSlot> _slots = new(); // 이따가 지우자
    private List<ItemSlotData> _slotData = new(); // 인벤토리에 포함된 모든 슬롯 리스트
    public GameObject slotPrefab; // 슬롯 프리팹
    private Transform _dropPosition;

    private int _selectedIndex = -1; // 현재 선택된 슬롯 인덱스 (-1은 선택되지 않음)

    enum GameObjects
    {
        SlotContainer
    }

    enum Texts
    {
        SelectedItemName,
        SelectedItemDescription,
        SelectedItemStatName,
        SelectedItemStatValue,
    }

    enum Buttons
    {
        UseButton,
        DropButton
    }
    
    /// 초기화 시 슬롯 생성 및 세팅
    public void Initialize(List<ItemSlotData> slotData, Transform dropPoint)
    {
        AutoBind<GameObject>(typeof(GameObjects));
        AutoBind<TextMeshProUGUI>(typeof(Texts));
        AutoBind<Button>(typeof(Buttons));

        _dropPosition = dropPoint;
        
        _slotData = slotData;
        
        Transform parent = GetObject((int)GameObjects.SlotContainer).transform;
        
        // 기존 슬롯 삭제
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
        _slots.Clear();
        
        // 새로운 슬롯 생성 및 추가
        for (int i = 0; i < _slotData.Count; i++)
        {
            var slotGo = Instantiate(slotPrefab, GetObject((int)GameObjects.SlotContainer).transform);
            var itemSlot = slotGo.GetComponent<ItemSlot>();
            itemSlot.Set(_slotData[i], this, i); // 아이템 없는 빈 슬롯으로 초기화
            _slotData[i].OnItemChanged += UpdateUI; // 이벤트 구독
            _slots.Add(itemSlot);
        }
        
        ClearSelectedItemWindow();
    }

    private void OnDisable()
    {
        foreach (var slot in _slotData)
        {
            slot.OnItemChanged -= UpdateUI; // 이벤트 해제
        }
    }


    //UI업데이트
    private void UpdateUI()
    {
        for (var i = 0; i < _slots.Count; i++)
        {
            _slots[i].Set(_slotData[i], this, i); // 아이템 없는 빈 슬롯으로 초기화
        }
        UpdateSelectedItemUI(_slotData[_selectedIndex]);
    }
    
    // 선택아이템 관련 UI  삭제
    private void ClearSelectedItemWindow()
    {
        GetText((int)Texts.SelectedItemName).text = string.Empty;
        GetText((int)Texts.SelectedItemDescription).text = string.Empty;
        GetText((int)Texts.SelectedItemStatName).text = string.Empty;
        GetText((int)Texts.SelectedItemStatValue).text = string.Empty;

        GetButton((int)Buttons.UseButton).gameObject.SetActive(false);
        GetButton((int)Buttons.DropButton).gameObject.SetActive(false);
    }

    
    // 아이템 삭제 (버림)
    private void ThrowItem(ItemData data)
    {
        if (data == null) return;
        Instantiate(data.DropPrefab, _dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }
    
    // 특정 슬롯의 아이템 제거
    public void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _slotData.Count)
        {
            _slotData[slotIndex].Clear();
            _slots[slotIndex].ClearUI();
        }
    }
    
    // 특정 슬롯을 선택
    public void SelectItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _slotData.Count)
        {
            // 이전 선택된 슬롯 비활성화
            if( _selectedIndex >= 0 && _selectedIndex < _slots.Count)
             _slots[_selectedIndex].UpdateSelected(false);

            // 새로운 슬롯 선택
            _selectedIndex = slotIndex;
            _slots[slotIndex].UpdateSelected(true);
            UpdateSelectedItemUI(_slotData[slotIndex]); // 선택된 아이템 관련 UI 업데이트
        }
    }
    
    /// 선택된 아이템 관련 UI 업데이트
    private void UpdateSelectedItemUI(ItemSlotData selectedItem)
    {
        if (!selectedItem.Item)
        {
            ClearSelectedItemWindow();
            return;
        }

        var strategy = GetStrategy(selectedItem.Item); // 전략 패턴 가져오기

        // 선택된 아이템의 정보를 UI에 반영

        GetText((int)Texts.SelectedItemName).text = selectedItem.Item.ItemName;
        GetText((int)Texts.SelectedItemDescription).text = selectedItem.Item.ItemDescription;
        strategy.UpdateStats(GetText((int)Texts.SelectedItemStatName)
            , GetText((int)Texts.SelectedItemStatValue), selectedItem.Item);

        // 버튼 관련 UI 업데이트
        UpdateButtons(strategy, selectedItem);
    }

    private void UpdateButtons(IItemTypeStrategyUI strategyUI, ItemSlotData selectedItem)
    {
        ConfigureUseButton(strategyUI, selectedItem);
        ConfigureDropButton();
    }

    private void ConfigureUseButton(IItemTypeStrategyUI strategyUI, ItemSlotData selectedItem)
    {
        Button useButton = GetButton((int)Buttons.UseButton);

        string buttonText = strategyUI.GetButtonText(selectedItem);
        UpdateButtonUI(useButton, buttonText);

        useButton.onClick.RemoveAllListeners();
        strategyUI.ConfigureButtonAction(useButton, selectedItem);
       // useButton.onClick.AddListener(RemoveSelectedItem);
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
    
    private void RemoveSelectedItem()
    {
        if(_slotData[_selectedIndex].RemoveSelectedItem())
            ClearSelectedItemWindow();
        
        _slots[_selectedIndex].UpdateUI();
    }

    private IItemTypeStrategyUI GetStrategy(ItemData item)
    {
        return item switch
        {
            UseItemData => new UseItemStrategyUI(),
            EquipItemData => new EquipItemStrategyUI(),
            _ => new EtcItemStrategyUI()
        };
    }

    /// <summary>
    /// "버리기" 버튼 클릭 이벤트
    /// </summary>
    private void OnDropButton()
    {
        ThrowItem(_slotData[_selectedIndex].Item);
        SignalManager.Instance.EmitSignal("OnPlayerUnEquip", _slotData[_selectedIndex]);
        RemoveSelectedItem();
    }
    
    // /// <summary>
    // /// 모든 슬롯 초기화
    // /// </summary>
    // public void ClearAllSlots()
    // {
    //     foreach (var slot in _slots)
    //         slot.ClearUI();
    // }
}