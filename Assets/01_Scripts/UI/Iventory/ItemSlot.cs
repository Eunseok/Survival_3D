using System.Globalization;
using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;
    public UIInventory inventory;
    public int index;
    public bool equipped;
    
    
    public Image icon;
    public TextMeshProUGUI quatityText;  // 수량표시 Text
    public Outline outline;             // 선택시 Outline 표시위한 컴포넌트

    public float quantity = 0;
    
    
    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    // UI(슬롯 한 칸) 업데이트를 위한 함수
    // 아이템데이터에서 필요한 정보를 각 UI에 표시
    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.Icon;
        quatityText.text = quantity > 1 ? quantity.ToString(CultureInfo.InvariantCulture) : string.Empty;

        if(outline != null)
        {
            outline.enabled = equipped;
        }
    }
    

    
    // UI(슬롯 한 칸)에 정보가 없을 때 UI를 비워주는 함수
    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }
    
    public void OnClickButton()
    {
       inventory.SelectItem(index);
    }
}