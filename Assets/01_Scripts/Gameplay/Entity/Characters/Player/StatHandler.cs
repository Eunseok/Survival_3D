using _01_Scripts.Gameplay.Entity.Characters.Player.Old;
using Managers;
using Scripts.Characters;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Header("플레이어 설정")] [Tooltip("캐릭터의 이동 속도 (m/s)")]
    public float MoveSpeed = 2.0f;

    [Tooltip("캐릭터의 질주(Sprint) 속도 (m/s)")] 
    public float SprintSpeed = 5.335f;
        
    [Tooltip("질주시 소모되는 스테미나")]
    public float SprintStaminaCost = 20f;
    
    [Tooltip("캐릭터의 등반(Climbing) 속도 (m/s)")] 
    public float ClimbSpeed = 3f;
        
    [Tooltip("등반시 소모되는 스테미나")]
    public float ClibStaminaCost = 20f;
    
    [Space(10)] [Tooltip("플레이어가 점프할 수 있는 최대 높이")]
    public float JumpHeight = 1.2f;
    
    [Header("Conditions")] 
    public Condition health;
     public Condition stamina;

    public float HealthValue => health.CurValue;
    public float StaminaValue => stamina.CurValue;
    


    public float HandleMovementSpeed(bool isSprinting)
    {
        if (!isSprinting) return MoveSpeed;
            
        bool canDash = SignalManager.Instance.EmitSignal<float, bool>(
            "OnUseStamina", SprintStaminaCost * Time.deltaTime);
        if (canDash)
            return SprintSpeed;
            
        return MoveSpeed;
    }

}