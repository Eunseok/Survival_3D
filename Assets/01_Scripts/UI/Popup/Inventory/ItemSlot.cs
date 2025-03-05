using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    // 슬롯 데이터 필드
    private ItemSlotData _itemData; // 슬롯의 아이템 데이터
    private PopupInventory _inventory; // 해당 슬롯이 포함된 인벤토리
    public int index; // 인벤토리 내 슬롯의 위치 인덱스

    private bool _isSelected; // 슬롯 선택 여부
    public bool IsSelected => _isSelected;


    // UI Elements
    [Header("UI Elements")] public Image icon; // 슬롯의 아이템 아이콘
    public TextMeshProUGUI quantityText; // 아이템 수량을 표시하는 텍스트
    public Outline outline; // 슬롯이 선택되었을 때 Outlining 처리를 위한 컴포넌트

    private void OnEnable()
    {
        // 슬롯 활성화/비활성화 시 Outline 처리
        UpdateSelected(_isSelected);
    }
    
    
    /// 슬롯의 데이터와 UI를 설정
    public void Set(ItemSlotData newItemData, PopupInventory parentInventory, int slotIndex)
    {
        _itemData = newItemData;
        _inventory = parentInventory;
        index = slotIndex;

        UpdateUI();
    }


    /// 슬롯 UI를 업데이트
    public void UpdateUI()
    {
        if (_itemData.Item != null)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = _itemData.Item.Icon;
            quantityText.text = _itemData.Quantity > 1 ? _itemData.Quantity.ToString(CultureInfo.InvariantCulture) : string.Empty;
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
    

    /// 슬롯 UI를 비우기
    public void ClearUI()
    {
       // _itemData.Item = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
        UpdateSelected(false); // 슬롯 선택 해제
    }

    /// <summary>
    /// 슬롯을 선택(강조 처리) 또는 선택 해제
    /// </summary>
    public void UpdateSelected(bool isSelected)
    {
        _isSelected = isSelected;
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