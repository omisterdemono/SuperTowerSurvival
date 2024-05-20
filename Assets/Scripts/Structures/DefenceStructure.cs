using System.Collections.Generic;
using System.Linq;
using Components;
using Mirror;
using UnityEngine;

namespace Structures
{
    public class DefenceStructure : Structure
    {
        [SerializeField] private Sprite[] _rotationSprites;
        [SerializeField] private GameObject _projectile;

        [SerializeField] private Transform _firePositionsHolder;
        [SerializeField] private Transform[] _firePositions;
        [SerializeField] private float _damage;
        [SerializeField] private float _cooldownSeconds;
        
        [SerializeField] private CustomTrigger _attackRange;

        [SyncVar] private float _rotateAngle;

        private List<Transform> _targetsInRange = new();
        private CooldownComponent _cooldownComponent;
        private int _rotateIndex;

        public new void Awake()
        {
            base.Awake();

            _cooldownComponent= new CooldownComponent() { CooldownSeconds = _cooldownSeconds };
            IsBeingBuilt = true;
        }

        public new void Start()
        {
            base.Start();

            _attackRange.EnteredTrigger += EnemyEnteredAttackRadius;
            _attackRange.ExitedTrigger += EnemyLeftAttackRadius;
        }

        public new void Update()
        {
            base.Update();
            if (!IsBeingBuilt)
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
            var closest = _targetsInRange?.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).First();
            return closest.position;
        }

        private void CountRotationIndex()
        {
            if (!isServer)
            {
                return;
            }
        
            if (_targetsInRange.Count == 0)
            {
                return;
            }

            var direction = (SelectClosestTarget() - transform.position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rotateAngle = angle < 0 ? 360.0f + angle : angle;
        }

        private void EnemyEnteredAttackRadius(Collider2D collision)
        {
            if (collision.TryGetComponent(out Enemy enemy) && !_targetsInRange.Contains(enemy.transform))
            {
                _targetsInRange.Add(enemy.transform);


                if (_targetsInRange.Count == 1)
                {
                    _rotateIndex = 0;
                }
            }
        }

        private void EnemyLeftAttackRadius(Collider2D collision)
        {
            if (collision.TryGetComponent<Enemy>(out Enemy enemy) && _targetsInRange.Contains(enemy.transform))
            {
                _targetsInRange.Remove(enemy.transform);

                if (_targetsInRange.Count == 0)
                {
                    _rotateIndex = -1;
                }
            }
        }

        public void FireBullet()
        {
            if (!isServer)
            {
                return;;
            }
        
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
}
