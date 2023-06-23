using Assets.Scripts.Weapons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IWeapon, IEquipable
{
    [SerializeField] private string[] _desiredTargets;
    [SerializeField] private float _damage;
    [SerializeField] private bool _needFlip;
    [SerializeField] private float _cooldown;

    public float Damage { get => _damage; set => _damage = value; }
    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }
    public bool NeedRotation { get; set; } = true;

    private Animator _animator;
    private List<HealthComponent> _targetsInRange = new List<HealthComponent>();

    private float _timeToNextHit;
    private bool _isAttacking;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Attack(Vector2 direction)
    {
        if (_timeToNextHit > 0)
        {
            return;
        }
        _timeToNextHit = _cooldown;

        ChangeAttackingState();
    }

    private void Update()
    {
        HandleCooldown();
    }

    public void DealDamage()
    {
        foreach (var enemyHealth in _targetsInRange)
        {
            enemyHealth.Damage(Damage);
        }

        ChangeAttackingState();
    }

    private void HandleCooldown()
    {
        if (_timeToNextHit == 0)
        {
            return;
        }

        if (_timeToNextHit < 0)
        {
            _timeToNextHit = 0;
            return;
        }

        _timeToNextHit -= Time.deltaTime;
    }

    public void ChangeAttackingState()
    {
        _isAttacking = !_isAttacking;
        _animator.SetBool("isAttacking", _isAttacking);
    }

    public void Hold(Vector2 direction)
    {
        Attack(direction);
    }

    public void KeyUp(Vector2 direction)
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
}
