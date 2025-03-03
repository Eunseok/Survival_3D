using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement playerMovement;
    
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    
}