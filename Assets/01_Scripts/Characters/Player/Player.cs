using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public ResourceController resourceController;
    public StatHandler statHandler;
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        
        playerMovement = GetComponent<PlayerMovement>();
        resourceController = GetComponent<ResourceController>();
        statHandler = GetComponent<StatHandler>();
    }
    
    
}