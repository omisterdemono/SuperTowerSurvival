using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class HealthComponent : NetworkBehaviour
{
    public Action OnDeath;

    [SerializeField] private float _maxHealth;
    
    private float _currentHealth;
    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }
    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
    }

    public void Heal(float healHP)
    {
        if (_currentHealth + healHP >= MaxHealth)
        {
            _currentHealth = MaxHealth;
            return;
        }
        _currentHealth += healHP;
    }

    public void Damage(float damageHP)
    {
        if (_currentHealth <= damageHP)
        {
            _currentHealth = 0;

            OnDeath.Invoke();
            return;
        }
        _currentHealth -= damageHP;
    }

    void Start()
    {
        _currentHealth = _maxHealth;
    }
}
