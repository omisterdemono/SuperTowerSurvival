using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SelfMovingTurretScript : NetworkBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private HealthComponent _healthComponent;
    [SyncVar] private uint _skill;
    private Transform _followingTarget;
    private Vector3 _velocity = Vector3.zero;

    [SerializeField] private Sprite[] _rotationSprites;
    [SerializeField] private GameObject _projectile;
    [Range(0f, 5f)]
    [SerializeField] private float _followRange = 3f;
    [Range(0f,1f)]
    [SerializeField] private float _smoothness = 0.5f;

    [SerializeField] private Transform _firePositionsHolder;
    [SerializeField] private Transform[] _firePositions;
    [SerializeField][SyncVar] private float _damage;
    [SerializeField][SyncVar] private float _cooldownSeconds;

    [SyncVar] private float _rotateAngle;

    private List<Transform> _targetsInRange = new();
    private CooldownComponent _cooldownComponent;
    private int _rotateIndex;

    public Transform FollowingTarget { get => _followingTarget; set => _followingTarget = value; }
    public uint Skill { get => _skill; set => _skill = value; }
    public float Damage { get => _damage; set => _damage = value; }
    public float CooldownSeconds { get => _cooldownSeconds; set => _cooldownSeconds = value; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _healthComponent = GetComponent<HealthComponent>();
        _healthComponent.OnDeath += Death;
    }

    public void SetTarget(uint targetid)
    {
        _cooldownComponent = new CooldownComponent() { CooldownSeconds = _cooldownSeconds };
        _followingTarget = NetworkServer.spawned[targetid].GetComponent<Transform>();
    }

    private void Death()
    {
        NetworkServer.spawned[_skill].GetComponent<SelfMovingTurretSkill>().Turrets.Remove(gameObject);
    }

    private void Update()
    {
        MoveToTargetInRange();
        RotateTowardsTarget();
        
        CountRotationIndex();

        _cooldownComponent.HandleCooldown();
        FireBullet();
    }

    [Server]
    private void MoveToTargetInRange()
    {
        if (_followingTarget == null)
            return;
        if (Vector3.Distance(transform.position, _followingTarget.position) > _followRange)
        {
            Vector3 posToTargetPos = (_followingTarget.position - transform.position).normalized;
            transform.position = Vector3.SmoothDamp(transform.position, _followingTarget.position - posToTargetPos * _followRange, ref _velocity, 0.5f);
        }
        if (_targetsInRange.Count == 0)
        {
            var direction = (_followingTarget.position - transform.position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rotateAngle = angle < 0 ? 360.0f + angle : angle;
        }
    }

    private void RotateTowardsTarget()
    {
        if (_rotateIndex == -1)
        {
            return;
        }

        _rotateIndex = (int)((_rotateAngle / 360.0f) * _rotationSprites.Length);
        _firePositionsHolder.rotation = Quaternion.Euler(0, 0, _rotateAngle);
        _spriteRenderer.sprite = _rotationSprites[_rotateIndex];
    }

    private Vector3 SelectClosestTarget()
    {
        var closest = _targetsInRange.OrderBy(t =>Vector3.Distance(transform.position, t.transform.position)).First();
        return closest.position;
    }

    [Server]
    private void CountRotationIndex()
    {
        if (_targetsInRange.Count == 0)
        {
            return;
        }

        var direction = (SelectClosestTarget() - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rotateAngle = angle < 0 ? 360.0f + angle : angle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy) && !_targetsInRange.Contains(enemy.transform) && collision is BoxCollider2D)
        {
            _targetsInRange.Add(enemy.transform);


            if (_targetsInRange.Count == 1)
            {
                _rotateIndex = 0;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy>(out Enemy enemy) && _targetsInRange.Contains(enemy.transform))
        {
            _targetsInRange.Remove(enemy.transform);
        }
    }

    [Server]
    public void FireBullet()
    {
        if (!_cooldownComponent.CanPerform || _targetsInRange.Count == 0)
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

        _cooldownComponent.ResetCooldown();
    }
}
