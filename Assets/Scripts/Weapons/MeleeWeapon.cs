using Assets.Scripts.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Components;
using Inventory.Models;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IWeapon, IEquipable
{
    [SerializeField] private string[] _desiredTargets;
    [SerializeField] private float _damage;
    [SerializeField] private bool _needFlip;
    [SerializeField] private float _cooldownSeconds;

    public float Damage { get => _damage; set => _damage = value; }

    public ItemSO Item
    {
        get => _item;
        set => _item = value;
    }

    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }
    public bool NeedRotation { get; set; } = true;
    public bool CanPerform => _cooldownComponent.CanPerform;
    public float CooldownSeconds => _cooldownSeconds;

    public static event EventHandler OnMeleeHit;
    public static event EventHandler OnMeleeMissed;

    public Vector3 MousePosition { get; set; }

    private Animator _animator;
    private List<HealthComponent> _targetsInRange = new List<HealthComponent>();
    private CooldownComponent _cooldownComponent;

    private bool _isAttacking;
    [SerializeField] private ItemSO _item;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _cooldownComponent = new CooldownComponent() { CooldownSeconds = _cooldownSeconds };
    }

    public void Attack()
    {
        if (!_cooldownComponent.CanPerform)
        {
            return;
        }
        _cooldownComponent.ResetCooldown();

        StartCoroutine(DelayedAttack());
    }

    private IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(_cooldownSeconds);
        DealDamage();
    }

    private void Update()
    {
        _cooldownComponent.HandleCooldown();
    }

    public void DealDamage()
    {
        bool isHit = false;
        foreach (var enemyHealth in _targetsInRange)
        {
            enemyHealth.Damage(Damage);
            if (!isHit)
            {
                isHit = true;
                OnMeleeHit?.Invoke(this, EventArgs.Empty);
                var knockback = enemyHealth.GetComponent<KnockbackComponent>();
                Vector2 direction =  enemyHealth.transform.position - this.transform.position;
                knockback?.PlayKnockback(direction.normalized, 15f, 0.3f);
            }
        }
        if (!isHit)
        {
            OnMeleeMissed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ChangeAnimationState()
    {
        _isAttacking = !_isAttacking;
        _animator.SetBool("isAttacking", _isAttacking);
    }

    public void Hold()
    {
        Attack();
    }

    public void KeyUp()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //saving for attack
        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if (component != null)
            {
                _targetsInRange.Add(component.GetComponent<HealthComponent>());
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //removing from attack
        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if (component != null)
            {
                _targetsInRange.Remove(component.GetComponent<HealthComponent>());
                return;
            }
        }
    }

    public void Interact()
    {
        Attack();
    }

    public void FinishHold()
    {
        KeyUp();
    }
}
