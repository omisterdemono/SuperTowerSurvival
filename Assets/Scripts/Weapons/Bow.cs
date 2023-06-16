using Assets.Scripts.Weapons;
using Mirror;
using UnityEngine;

public class Bow : MonoBehaviour, IAttacker, IEquipable
{
    [Header("Required setup")]
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private bool _needFlip;

    [Header("Shoot parameters")]
    [SerializeField] private float _damage = 1.0f;
    [SerializeField] private float _minProgressToShotSeconds;
    [SerializeField] private float _maxChargeSeconds;
    [SerializeField] private float _cooldownSeconds;

    public float Damage { get => _damage; set => _damage = value; }
    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }

    private float _chargeProgressSeconds;
    private float _timeToNextShot;

    public void Attack(Vector2 direction)
    {
        return;
    }

    private void Update()
    {
        HandleCooldown();
    }

    private void HandleCooldown()
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

    private void HandleCharge()
    {
        if (_chargeProgressSeconds > _maxChargeSeconds)
        {
            _chargeProgressSeconds = _maxChargeSeconds;
        }

        _chargeProgressSeconds += Time.deltaTime;
    }

    private void FireArrow(Vector2 direction)
    {
        if (_timeToNextShot != 0 || _chargeProgressSeconds < _minProgressToShotSeconds)
        {
            _chargeProgressSeconds = 0;
            return;
        }

        GameObject projectile = Instantiate(_projectile, _firePosition.position, _firePosition.rotation);
        var projectileComponent = projectile.GetComponent<Projectile>();

        projectileComponent.Speed *= _chargeProgressSeconds / _maxChargeSeconds;
        projectileComponent.Direction = _firePosition.right;
        
        NetworkServer.Spawn(projectile);

        _chargeProgressSeconds = 0;
        _timeToNextShot = _cooldownSeconds;
    }

    public void Hold(Vector2 direction)
    {
        HandleCharge();
    }

    public void KeyUp(Vector2 direction)
    {
        FireArrow(direction);
    }
}
