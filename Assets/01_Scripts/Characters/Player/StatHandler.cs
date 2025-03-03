using UnityEngine;

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
    [SerializeField]
    private Condition health;
    [SerializeField]
    private Condition hunger;
    [SerializeField]
    private Condition stamina;

    public Condition Health => health;
    public float HealthValue
    {
        get => health.curValue;
        set => health.curValue = value;
    }

    public Condition Hunger => hunger;
    public float HungerValue
    {
        get => hunger.curValue;
        set => hunger.curValue = value;
    }

    public Condition Stamina => stamina;
    public float StaminaValue
    {
        get => stamina.curValue;
        set => stamina.curValue = value;
    }

}