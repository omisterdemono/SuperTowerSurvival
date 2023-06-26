using UnityEngine;
using Mirror;
using System;

public class HealthComponent : NetworkBehaviour
{
    public Action OnCurrentHealthChanged;
    public Action OnDeath;

    [SerializeField] private float _maxHealth;

    [SyncVar(hook = nameof(CurrentHealthHook))] private float _currentHealth;

    private void CurrentHealthHook(float oldValue, float newValue)
    {
        OnCurrentHealthChanged?.Invoke();
    }

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
        if (!isLocalPlayer)
        {
            _currentHealth = _maxHealth;
        }
    }
}
