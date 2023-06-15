using Mirror;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Spawns bullet, rotates parent object for all weapons
/// towards cursor.
/// </summary>
public class FireWeapon : MonoBehaviour, IAttacker
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform[] _firePositions;
    [SerializeField] private float _damage = 1.0f;
    [SerializeField] private float _cooldownSeconds;

    private float _timeToNextShot;

    private SpriteRenderer _spriteRenderer;
    private Transform _equippedSlot;


    public float Damage { get; set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _equippedSlot = transform.parent;
    }

    private void Update()
    {
        HandleFireRate();
    }

    private void HandleFireRate()
    {
        if (_timeToNextShot == 0)
        {
            return;
        }

        if (_timeToNextShot < 0)
        {
            _timeToNextShot = 0;
            return;
        }

        _timeToNextShot -= Time.deltaTime;
    }

    public void Attack(Vector2 direction, ref Action<Vector2> performAttack)
    {
        FireBullet(direction);
    }

    public void FireBullet(Vector2 direction)
    {
        if (_timeToNextShot != 0)
        {
            return;
        }

        foreach (var firePosition in _firePositions)
        {
            GameObject projectile = Instantiate(_projectile, firePosition.position, _equippedSlot.rotation);
            var bulletComponent = projectile.GetComponent<Projectile>();
            bulletComponent.Direction = direction;
            bulletComponent.Damage = _damage;
            NetworkServer.Spawn(projectile);
        }

        _timeToNextShot = _cooldownSeconds;
    }
}
