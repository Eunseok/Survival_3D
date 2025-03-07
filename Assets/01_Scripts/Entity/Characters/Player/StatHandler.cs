using System;
using System.Collections;
using Scripts.Characters;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Header("Movement")] 
    [Range(1f, 20f)] [SerializeField] private float normalSpeed = 5;
    public float Speed
    {
        get => normalSpeed;
        set => normalSpeed = Mathf.Clamp(value, 0, 100);
    }
    
    [Range(1f, 20f)] [SerializeField] private float dashSpeed = 5;
    public float DashSpeed
    {
        get => dashSpeed;
        set => dashSpeed = Mathf.Clamp(value, 0, 100);
    }
 
    

    [Range(50f, 200f)] [SerializeField] private float jumpForce = 80;

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = Mathf.Clamp(value, 5, 80);
    }


    [Header("Conditions")] 
    public Condition health;
    public Condition hunger;
     public Condition stamina;

    public float HealthValue => health.CurValue;
    public float HungerValue => hunger.CurValue;
    public float StaminaValue => stamina.CurValue;
    



}