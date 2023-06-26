using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class DefenseStructure : Structure
{
    [SerializeField] private Sprite[] _rotationSprites;
    [SerializeField] private GameObject _projectile;

    [SerializeField] private Transform _firePositionsHolder;
    [SerializeField] private Transform[] _firePositions;
    [SerializeField] private float _damage;
    [SerializeField] private float _cooldownSeconds;

    [SyncVar] private float _rotateAngle;

    private Transform _currentTarget;
    private int _rotateIndex;
    private float _timeToNextShot;

    public new void Awake()
    {
        base.Awake();
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
        HandleFireRate();
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

    [Server]
    private void CountRotationIndex()
    {
        if (_currentTarget == null)
        {
            return;
        }

        var direction = (_currentTarget.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rotateAngle = angle < 0 ? 360.0f + angle : angle;
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

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        //handling enemies that entered the attack radius
        if (collision.TryGetComponent(out Enemy enemy) && _currentTarget == null)
        {
            _currentTarget = enemy.transform;
            _rotateIndex = 0;
        }
    }

    private new void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        //removing enemy that left the attack radius
        if (collision.TryGetComponent<Enemy>(out Enemy enemy) && enemy.transform == _currentTarget)
        {
            _currentTarget = null;
            _rotateIndex = -1;
        }
    }

    public void FireBullet()
    {
        if (_timeToNextShot != 0 || _currentTarget == null)
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
}
