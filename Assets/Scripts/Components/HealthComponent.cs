using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;

public class HealthComponent : NetworkBehaviour
{
    public Action OnCurrentHealthChanged;
    public Action OnDeath;

    [SerializeField] private float _maxHealth;

    [SyncVar(hook = nameof(CurrentHealthHook))]
    private float _currentHealth;

    private void CurrentHealthHook(float oldValue, float newValue)
    {
        OnCurrentHealthChanged?.Invoke();
    }

    [SerializeField] private GameObject _healthBar;

    [SyncVar] private float _hpRatio;
    private CanvasGroup _canvasGroupHB;
    private Image _imageHP;

    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            //_currentHealth = value;
            ChangeHealth(value);
            _hpRatio = _currentHealth / _maxHealth;
            if (_imageHP != null)
            {
                _imageHP.fillAmount = _hpRatio;
            }

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
    //[Server]
    public void ChangeHealth(float health)
    {
        _currentHealth = health;
    }

    [Server]
    private void InitHealth()
    {
        ChangeHealth(MaxHealth);
    }

    private void Awake()
    {
        InitHealth();

        if (_healthBar != null)
        {
            _canvasGroupHB = _healthBar.GetComponentInChildren<CanvasGroup>();
            _imageHP = _canvasGroupHB.transform.GetChild(1).GetComponentInChildren<Image>();
        }
    }
}