using System;
using UnityEngine;


public class ResourceController : MonoBehaviour
{
    private StatHandler statHandler;
    public event Action onTakeDamage;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
    }

    private void Update()
    {
        statHandler.Hunger.PassiveApply();
        statHandler.Stamina.PassiveApply();
        
        if(statHandler.HungerValue < 0f)
        {
            statHandler.Health.PassiveApply();
        }
    
        if( statHandler.HealthValue < 0f)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        statHandler.HealthValue += amount;
        onTakeDamage?.Invoke();
    }
    
    public void Eat(float amount)
    {
        statHandler.HungerValue += amount;
    }
    
    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }
}