using System;
using System.Collections;
using UnityEngine;


public interface IDamageable
{
    void TakeDamage(float damage);
}

public class ResourceController : MonoBehaviour, IDamageable
{
    private StatHandler _statHandler;
    public event Action OnTakeDamage;
    
    private bool _isSpeedBoosted;

    private void Awake()
    {
        _statHandler = GetComponent<StatHandler>();
        
        SignalManager.Instance.ConnectSignal<float>("OnPlayerHeal", Heal);
        SignalManager.Instance.ConnectSignal<float, bool>("OnUseStamina", UseStamina);
        
        SignalManager.Instance.ConnectSignal<float>("OnPlayerHit", TakeDamage);
        SignalManager.Instance.ConnectSignal<float, float>("OnApplySpeedBuff", ApplySpeedBuff);
    }

    private void Update()
    {
        _statHandler.stamina.PassiveApply();
    
        if( _statHandler.HealthValue <= 0f)
        {
            Die();
        }
    }

    private void Heal(float amount)
    {
        _statHandler.health.Apply(amount);
        
        if(amount < 0f)
             OnTakeDamage?.Invoke();
    }
    

    private bool UseStamina(float amount)
    {
        if(_statHandler.stamina.CurValue - amount < 0)
        {
            return false;
        }
        _statHandler.stamina.Apply(-amount);
        return true;
    }
    
    private void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakeDamage(float damage)
    {
        _statHandler.health.Apply(-damage);
        OnTakeDamage?.Invoke(); 
        if( _statHandler.HealthValue <= 0f)
        {
            Die();
        }
    }

    private void ApplySpeedBuff(float boostAmount, float duration)
    {
        if (!_isSpeedBoosted)
        {
           // StartCoroutine(SpeedBuffRoutine(boostAmount, duration));
        }
    }

    // private IEnumerator SpeedBuffRoutine(float boostAmount, float duration)
    // {
    //     _isSpeedBoosted = true;
    //     _statHandler.Speed += boostAmount; // 속도 증가
    //     yield return new WaitForSeconds(duration); // 버프 지속 시간 대기
    //     _statHandler.Speed -= boostAmount; // 원래 속도로 복구
    //     _isSpeedBoosted = false;
    // }

}