using System;
using UnityEngine;


public interface IDamageable
{
    void TakeDamage(float damage);
}

public class ResourceController : MonoBehaviour, IDamageable
{
    private StatHandler _statHandler;
    public event Action OnTakeDamage;

    private void Awake()
    {
        _statHandler = GetComponent<StatHandler>();
        
        SignalManager.Instance.ConnectSignal<float>("OnPlayerHeal", Heal);
        SignalManager.Instance.ConnectSignal<float>("OnPlayerEat", Eat);
    }

    private void Update()
    {
        _statHandler.hunger.PassiveApply();
        _statHandler.stamina.PassiveApply();
        
        if(_statHandler.HungerValue <= 0f)
        {
            _statHandler.health.PassiveApply();
        }
    
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

    private void Eat(float amount)
    { 
        _statHandler.hunger.Apply(amount);
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
}