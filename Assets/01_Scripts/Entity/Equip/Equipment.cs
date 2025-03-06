using Scripts.Items;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;
    private ItemSlotData _curSlot;


    private void OnEnable()
    {
        SignalManager.Instance.ConnectSignal<ItemSlotData>("OnPlayerEquip", EquipNew);
        SignalManager.Instance.ConnectSignal<ItemSlotData>("OnPlayerUnEquip", UnEquip);
        
    }

    private void EquipNew(ItemSlotData data)
    {
        UnEquip(_curSlot);

        GameObject obj = (data.Item as EquipItemData)?.equipPrefab;
        curEquip = Instantiate(obj, equipParent).GetComponent<Equip>();
        data.IsEquipped = true;
        _curSlot = data;
    }

    private void UnEquip(ItemSlotData data)
    {
        if(curEquip != null && _curSlot == data)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
            data.IsEquipped = false;
        }
    }
}