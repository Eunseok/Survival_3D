using System;
using Scripts.Characters;
using Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public ResourceController resourceController;
    public StatHandler statHandler;

    public Action<ItemData> AddItem;
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        
        playerMovement = GetComponent<PlayerMovement>();
        resourceController = GetComponent<ResourceController>();
        statHandler = GetComponent<StatHandler>();
    }
    
    
}