using System;
using System.Collections;
using Scripts.Characters;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Header("Movement")] 
    [Range(1f, 20f)] [SerializeField] private float normalSpeed = 5;
    private bool _isSpeedBoosted;



    public float Speed
    {
        get => normalSpeed;
        set => normalSpeed = Mathf.Clamp(value, 0, 100);
    }

    [Range(50f, 80f)] [SerializeField] private float jumpForce = 80;

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
    

    public void ApplySpeedBuff(float boostAmount, float duration)
    {
        if (!_isSpeedBoosted)
        {
            StartCoroutine(SpeedBuffRoutine(boostAmount, duration));
        }
    }

    private IEnumerator SpeedBuffRoutine(float boostAmount, float duration)
    {
        _isSpeedBoosted = true;
        Speed += boostAmount; // 속도 증가
        yield return new WaitForSeconds(duration); // 버프 지속 시간 대기
        Speed -= boostAmount; // 원래 속도로 복구
        _isSpeedBoosted = false;
    }


}