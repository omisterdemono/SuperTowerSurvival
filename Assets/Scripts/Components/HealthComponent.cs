using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class HealthComponent : NetworkBehaviour
{
    public Action OnCurrentHealthChanged;
    public Action OnDeath;

    [SerializeField] private float _maxHealth;
    
    [SyncVar] private float _currentHealth;

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
        set
        {
            _currentHealth = value;
            ChangeHealth(value);
            OnCurrentHealthChanged?.Invoke();
        }
    }

    public void Heal(float healHP)
    {
        if (CurrentHealth + healHP >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
            return;
        }
        CurrentHealth += healHP;
    }

    public void Damage(float damageHP)
    {
        if (CurrentHealth <= damageHP)
        {
            CurrentHealth = 0;

            OnDeath?.Invoke();
            return;
        }
        CurrentHealth -= damageHP;
    }

    [Command(requiresAuthority = false)]
    public void ChangeHealth(float health)
    {
        _currentHealth = health;
    }

    void Start()
    {
        CurrentHealth = _maxHealth;
    }
}
