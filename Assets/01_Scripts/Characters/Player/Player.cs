using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public ResourceController condition;
    public StatHandler statHandler;
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        
        playerMovement = GetComponent<PlayerMovement>();
        condition = GetComponent<ResourceController>();
        statHandler = GetComponent<StatHandler>();
    }
    
    
}