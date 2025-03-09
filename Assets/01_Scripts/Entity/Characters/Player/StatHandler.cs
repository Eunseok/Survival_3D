using Scripts.Characters;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float normalSpeed = 200f;

    public float Speed
    {
        get => normalSpeed;
        set => normalSpeed = value;
    }

    [SerializeField] private float dashSpeed = 300f;
    public float DashSpeed => dashSpeed;
        
     [SerializeField] private float climbSpeed = 100f;
    public float ClimbSpeed => climbSpeed;

    [SerializeField] private float jumpForce = 150f;
    public float JumpForce => jumpForce;


    [Header("Conditions")] 
    public Condition health;
    public Condition hunger;
     public Condition stamina;

    public float HealthValue => health.CurValue;
    public float HungerValue => hunger.CurValue;
    public float StaminaValue => stamina.CurValue;
    



}