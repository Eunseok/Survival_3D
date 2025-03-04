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
    
    public void Heal(float amount)
    {
        _statHandler.health.Apply(amount);
        OnTakeDamage?.Invoke();
    }
    
    public void Eat(float amount)
    { 
        _statHandler.health.Apply(amount);
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