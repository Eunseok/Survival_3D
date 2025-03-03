using UnityEngine;
using UnityEngine.Serialization;

public class StatHandler : MonoBehaviour
{
    [Header("Movement")] [Range(1f, 20f)] [SerializeField]
    private float speed = 5;

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, 20);
    }

    [Range(50f, 80f)] [SerializeField] private float jumpForce = 80;

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = Mathf.Clamp(value, 5, 80);
    }


    [Header("Conditions")] 
    public Condition Health;
    public Condition Hunger;
    public Condition Stamina;

    public float HealthValue => Health.CurValue;
    public float HungerValue => Hunger.CurValue;
    public float StaminaValue => Stamina.CurValue;

}