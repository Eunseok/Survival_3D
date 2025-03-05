using System.Globalization;
using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    // 슬롯 데이터 필드
    public ItemData item; // 슬롯의 아이템 데이터
    private UIInventory _inventory; // 해당 슬롯이 포함된 인벤토리
    public int index; // 인벤토리 내 슬롯의 위치 인덱스

    private bool _isSelected; // 슬롯 선택 여부
    public bool IsSelected => _isSelected;

    private bool _isEquipped;
    public bool IsEquipped => _isEquipped;

    // UI Elements
    [Header("UI Elements")] public Image icon; // 슬롯의 아이템 아이콘
    public TextMeshProUGUI quantityText; // 아이템 수량을 표시하는 텍스트
    public Outline outline; // 슬롯이 선택되었을 때 Outlining 처리를 위한 컴포넌트

    private float _quantity; // 아이템의 현재 수량

    public bool CanStack
    {
        get
        {
            if (item == null) return false;
            if (item is IStackable stackable)
                return _quantity < stackable.MaxStackAmount;
            return false;
        }
    }

    private void OnEnable()
    {
        // 슬롯 활성화/비활성화 시 Outline 처리
        UpdateEquippedStatus(_isSelected);
    }

    /// <summary>
    /// 슬롯의 데이터와 UI를 설정
    /// </summary>
    public void Set(ItemData newItem, float newQuantity, UIInventory parentInventory, int slotIndex)
    {
        item = newItem;
        _quantity = newQuantity;
        _inventory = parentInventory;
        index = slotIndex;

        UpdateUI();
    }

    /// <summary>
    /// 새로운 아이템 추가
    /// </summary>
    public void AddNewItem(ItemData newItem, float newQuantity)
    {
        item = newItem;
        _quantity = newQuantity;
        UpdateUI();
    }

    /// <summary>
    /// 아이템 스택 수량 추가
    /// </summary>
    public void AddStackItem(float amount = 1)
    {
        _quantity += amount;
        UpdateUI();
    }

    /// <summary>
    /// 슬롯 UI를 업데이트
    /// </summary>
    private void UpdateUI()
    {
        if (item != null)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = item.Icon;
            quantityText.text = _quantity > 1 ? _quantity.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
        else
        {
            ClearUI();
        }

        if (outline != null)
        {
            outline.enabled = _isSelected;
        }
    }

    /// <summary>
    /// 선택된 아이템 제거
    /// </summary>
    public void RemoveSelectedItem()
    {
        _quantity--;

        if (_quantity <= 0)
        {
            if (IsEquipped)
            {
                // UnEquip(selectedItemIndex);
            }

            ClearUI();
        }

        UpdateUI();
    }

    /// <summary>
    /// 슬롯 UI를 비우기
    /// </summary>
    public void ClearUI()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
        _inventory.UpdateSelectedItemUI(this);
        UpdateEquippedStatus(false); // 슬롯 선택 해제
    }

    /// <summary>
    /// 슬롯의 수량을 업데이트
    /// </summary>
    public void UpdateQuantity(float newQuantity)
    {
        _quantity = newQuantity;
        quantityText.text = _quantity > 1 ? _quantity.ToString(CultureInfo.InvariantCulture) : string.Empty;
    }

    /// <summary>
    /// 슬롯을 선택(강조 처리) 또는 선택 해제
    /// </summary>
    public void UpdateEquippedStatus(bool isEquipped)
    {
        _isSelected = isEquipped;
        if (outline != null)
        {
            outline.enabled = _isSelected;
        }
    }

    /// <summary>
    /// 슬롯 클릭 시 호출
    /// </summary>
    public void OnClickButton()
    {
        if (_inventory != null)
        {
            _inventory.SelectItem(index); // 클릭 시 부모 인벤토리에 선택된 아이템 인덱스를 전달
        }
    }
}