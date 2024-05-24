using Mirror;
using System;
using UnityEngine;
using System.Collections;
using Components;

/// <summary>
/// This class is responsible for attacking enemy entities,
/// but if needed, can be extended to attack something else.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : NetworkBehaviour
{
    public enum Type
    {
        RifleBullet,
        ShotgunBullet,
        CannonBullet,
        Arrow,
        EnemyArrow
    }

    [SerializeField] private string[] _desiredTargets;
    [SerializeField] private Type _owner;

    [SerializeField] private float _timeToFlyInSeconds;
    [SerializeField][SyncVar] private float _speed;
    
    [SerializeField] public Type type;

    [SyncVar] private float _damage;
    [SyncVar] private Vector2 _direction;

    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public Vector2 Direction { get => _direction; set => _direction = value; }

    public static event EventHandler OnAnyProjectileHit;
    public event Action<Collider2D> OnProjectileHit;

    private void Start()
    {
        StartCoroutine(Fly());
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(Direction * Speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is not BoxCollider2D)
        {
            return;
        }

        //hitting all desired targets
        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if(component != null) 
            {
                OnAnyProjectileHit?.Invoke(this, EventArgs.Empty);
                OnProjectileHit?.Invoke(collision);
                component.GetComponent<HealthComponent>().Damage(Damage);
                var knockback = component.GetComponent<KnockbackComponent>();
                knockback?.PlayKnockback(this.Direction, 5f, 0.15f);
                DestroyProjectile();
                return;
            }
        }
    }

    private IEnumerator Fly()
    {
        yield return new WaitForSeconds(_timeToFlyInSeconds);

        DestroyProjectile();
    }

    [Command(requiresAuthority = false)]
    private void DestroyProjectile()
    {
        NetworkServer.Destroy(gameObject);
    }
}
