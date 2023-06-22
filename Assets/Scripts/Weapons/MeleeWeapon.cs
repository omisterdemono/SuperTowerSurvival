using Assets.Scripts.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IAttacker, IEquipable
{
    [SerializeField] private string[] _desiredTargets;
    [SerializeField] private float _damage;
    [SerializeField] private bool _needFlip;

    public float Damage { get => _damage; set => _damage = value; }
    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Attack(Vector2 direction)
    {
        _animator.SetBool("isAttacking", true);
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
        //hitting all desired targets
        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if (component != null)
            {
                component.GetComponent<HealthComponent>().Damage(Damage);
                return;
            }
        }
    }
}
