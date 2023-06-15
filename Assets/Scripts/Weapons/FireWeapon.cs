using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Spawns bullet, rotates parent object for all weapons
/// towards cursor.
/// </summary>
public class FireWeapon : MonoBehaviour, IAttacker
{
    [SerializeField] private float _damage = 1.0f;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private float _bulletSpeed;
    
    private SpriteRenderer _spriteRenderer;
    private Transform _equippedSlot;

    public bool flip;

    public float Damage { get; set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _equippedSlot = transform.parent;
    }

    public void Attack(Vector2 direction, ref Action<Vector2> performAttack)
    {
        FireBullet(direction);
    }

    public void FireBullet(Vector2 direction)
    {
        GameObject projectile = Instantiate(_projectile, _firePosition.position, transform.rotation);
        var bulletComponent = projectile.GetComponent<Projectile>();
        bulletComponent.Direction = direction;

        NetworkServer.Spawn(projectile);
    }
}
