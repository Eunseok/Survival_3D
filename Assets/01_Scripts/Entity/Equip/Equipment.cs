using Scripts.Items;
using Unity.VisualScripting;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;


    private void OnEnable()
    {
        // SignalManager.Instance.ConnectSignal<EquipItemData>("OnPlayerEquip", EquipNew);
        // SignalManager.Instance.ConnectSignal("OnPlayerUnEquip", UnEquip);
        //
    }

    public void EquipNew(EquipItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }
}