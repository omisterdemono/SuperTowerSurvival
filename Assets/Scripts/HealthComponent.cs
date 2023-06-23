using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthComponent : NetworkBehaviour
{
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
        if (_currentHealth + healHP >= healHP)
        {
            _currentHealth = healHP;
            return;
        }
        _currentHealth += healHP;
    }

    public void Damage(float damageHP)
    {
        if (_currentHealth < damageHP)
        {
            _currentHealth = 0;
            return;
        }
        _currentHealth -= damageHP;
    }

    void Start()
    {
        _currentHealth = _maxHealth;
    }
}
