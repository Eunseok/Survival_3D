using Scripts.Characters;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Header("Conditions")] 
    public Condition health;
     public Condition stamina;

    public float HealthValue => health.CurValue;
    public float StaminaValue => stamina.CurValue;
    



}