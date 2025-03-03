using System;
using UnityEngine;


public interface IDamageable
{
    void TakeDamage(float damage);
}

public class ResourceController : MonoBehaviour, IDamageable
{
    private StatHandler statHandler;
    public event Action OnTakeDamage;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
    }

    private void Update()
    {
        statHandler.Hunger.PassiveApply();
        statHandler.Stamina.PassiveApply();
        
        if(statHandler.HungerValue <= 0f)
        {
            statHandler.Health.PassiveApply();
        }
    
        if( statHandler.HealthValue <= 0f)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        statHandler.Health.Apply(amount);
        OnTakeDamage?.Invoke();
    }
    
    public void Eat(float amount)
    { 
        statHandler.Health.Apply(amount);
    }
    
    private void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakeDamage(float damage)
    {
        statHandler.Health.Apply(-damage);
        OnTakeDamage?.Invoke(); 
        if( statHandler.HealthValue <= 0f)
        {
            Die();
        }
    }
}