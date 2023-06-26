using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;

public class HealthComponent : NetworkBehaviour
{
    public Action OnCurrentHealthChanged;
    public Action OnDeath;

    [SerializeField] private float _maxHealth;
    [SerializeField] private GameObject _healthBar;

    [SyncVar] private float _currentHealth;
    private CanvasGroup _canvasGroupHB;
    private Image _imageHP;

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
        if (_imageHP != null)
        {
            _imageHP.fillAmount = _currentHealth / _maxHealth;
        }
    }

    void Start()
    {
        CurrentHealth = _maxHealth;
        if (_healthBar != null)
        {
            _canvasGroupHB = _healthBar.GetComponentInChildren<CanvasGroup>();
            _imageHP = _canvasGroupHB.transform.GetChild(1).GetComponentInChildren<Image>();
        }
    }
}
