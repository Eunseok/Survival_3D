using System;
using Scripts.Characters;
using Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [FormerlySerializedAs("resourceController")] public ResourceHandler ResourceHandler;
    public StatHandler statHandler;
   // public InventorySystem playerInventory;
    
    public Action<ItemData> AddItem;
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        
        ResourceHandler = GetComponent<ResourceHandler>();
        statHandler = GetComponent<StatHandler>();
        //playerInventory = GetComponent<InventorySystem>();
    }
    
    
}