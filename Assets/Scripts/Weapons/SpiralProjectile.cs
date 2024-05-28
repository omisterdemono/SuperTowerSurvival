using Mirror;
using System;
using UnityEngine;
using System.Collections;
using Components;
using UnityEngine.UIElements;
using static UnityEngine.Mathf;

public class SpiralProjectile : NetworkBehaviour
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

    [SerializeField] private float _timeToFlyInSeconds = 7;
    [SerializeField][SyncVar] private float _speed = 2;
    [SerializeField][SyncVar] private float _angularSpeed = 50;
    [SerializeField][SyncVar] private float _damage = 10;

    [SerializeField] public Type type;

    //[SyncVar] private float _damage;
    [SyncVar] private Vector2 _direction;

    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public float AngularSpeed { get => _angularSpeed; set => _angularSpeed = value; }
    public Vector2 Direction
    {
        get => _direction; set
        {
            _direction = value;
            _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        }
    }

    public static event EventHandler OnAnyProjectileHit;
    public event Action<Collider2D> OnProjectileHit;

    private float _timePassed;
    private float _angle;
    private Vector3 _spawnPos;

    private void Start()
    {
        _timePassed = 0;
        //_angle = Direction.
        _spawnPos = transform.position;
        StartCoroutine(Fly());
    }

    private void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;
        _timePassed += deltaTime;

        Vector3 newPos = Vector3.zero;
        var radius = _speed * _timePassed;
        _angle += _angularSpeed * deltaTime;

        Vector3 deltaPos = new(radius * Cos(_angle * Mathf.Deg2Rad), radius * Sin(_angle * Mathf.Deg2Rad));

        transform.position = _spawnPos + deltaPos;

        float rotationAngle = Atan2(deltaPos.y, deltaPos.x) * Rad2Deg;
        if (AngularSpeed > 0) rotationAngle += 90;
        else rotationAngle -= 90;

        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is not BoxCollider2D)
        {
            return;
        }

        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if (component != null)
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
