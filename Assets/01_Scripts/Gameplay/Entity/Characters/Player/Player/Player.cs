using System;
using Scripts.Characters;
using Scripts.Items;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ResourceController resourceController;
    public StatHandler statHandler;
   // public InventorySystem playerInventory;
    
    public Action<ItemData> AddItem;
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        
        resourceController = GetComponent<ResourceController>();
        statHandler = GetComponent<StatHandler>();
        //playerInventory = GetComponent<InventorySystem>();
    }
    
    
}