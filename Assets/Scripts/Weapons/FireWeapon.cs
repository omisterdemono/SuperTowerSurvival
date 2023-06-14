using Mirror;
using System;
using UnityEngine;

/// <summary>
/// Spawns bullet, rotates parent object for all weapons
/// towards cursor.
/// </summary>
public class FireWeapon : MonoBehaviour, IAttacker
{
    [SerializeField] private float _damage = 1.0f;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePosition;
    
    private SpriteRenderer _spriteRenderer;

    public float Damage { get; set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack(Vector2 direction, ref Action<Vector2> performAttack)
    {
        performAttack = FireBullet;
    }

    public void FireBullet(Vector2 direction)
    {
        GameObject projectile = Instantiate(_projectile, _firePosition.position, transform.rotation);
        NetworkServer.Spawn(projectile);

        projectile.GetComponent<Rigidbody2D>().velocity = direction;
    }

    public void Rotate(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        _spriteRenderer.flipY = !(angle <= 90.0f && angle >= -90.0f);
    }
}
