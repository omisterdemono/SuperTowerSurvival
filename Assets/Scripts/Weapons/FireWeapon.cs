using Assets.Scripts.Weapons;
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
public class FireWeapon : MonoBehaviour, IAttacker, IEquipable
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform[] _firePositions;
    [SerializeField] private float _damage = 1.0f;
    [SerializeField] private float _cooldownSeconds;
    [SerializeField] private bool _needFlip;

    private float _timeToNextShot;
    private Transform _equippedSlot;


    public float Damage { get; set; }
    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }

    private void Awake()
    {
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

    public void Attack(Vector2 direction)
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
            GameObject projectile = Instantiate(_projectile, firePosition.position, firePosition.rotation);
            var bulletComponent = projectile.GetComponent<Projectile>();
            bulletComponent.Direction = firePosition.right;
            bulletComponent.Damage = _damage;
            NetworkServer.Spawn(projectile);
        }

        _timeToNextShot = _cooldownSeconds;
    }

    public void Hold(Vector2 direction)
    {
        FireBullet(direction);
    }

    public void KeyUp(Vector2 direction)
    {
        return;
    }
}
