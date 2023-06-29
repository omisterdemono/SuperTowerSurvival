using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefenseStructure : Structure
{
    [SerializeField] private Sprite[] _rotationSprites;
    [SerializeField] private GameObject _projectile;

    [SerializeField] private Transform _firePositionsHolder;
    [SerializeField] private Transform[] _firePositions;
    [SerializeField] private float _damage;
    [SerializeField] private float _cooldownSeconds;

    [SyncVar] private float _rotateAngle;

    private List<Transform> _targetsInRange = new();
    private CooldownComponent _cooldownComponent;
    private int _rotateIndex;

    public new void Awake()
    {
        base.Awake();

        _cooldownComponent= new CooldownComponent();

        Debug.Log("remove this line");
        IsBuilt = true;
    }

    public new void Update()
    {
        base.Update();
        if (!IsBuilt)
        {
            return;
        }

        RotateTowardsTarget();
        CountRotationIndex();

        _cooldownComponent.HandleCooldown();
        FireBullet();
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
        var closest = _targetsInRange.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).First();
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

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        //handling enemies that entered the attack radius
        if (collision.TryGetComponent(out Enemy enemy) && !_targetsInRange.Contains(enemy.transform))
        {
            _targetsInRange.Add(enemy.transform);


            if (_targetsInRange.Count == 1)
            {
                _rotateIndex = 0;
            }
        }
    }

    private new void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        //removing enemy that left the attack radius
        if (collision.TryGetComponent<Enemy>(out Enemy enemy) && _targetsInRange.Contains(enemy.transform))
        {
            _targetsInRange.Remove(enemy.transform);

            if (_targetsInRange.Count == 0)
            {
                _rotateIndex = -1;
            }
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
