using Assets.Scripts.Weapons;
using Mirror;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon, IEquipable
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

    private Animator _animator;

    public float Damage { get => _damage; set => _damage = value; }
    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }
    public bool NeedRotation { get; set; } = true;

    private bool _isCharging = false;
    private float _chargeProgressSeconds;
    private float _timeToNextShot;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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

        if (!_isCharging)
        {
            ChangeChargeAnimationState();
        }
        _chargeProgressSeconds += Time.deltaTime;
    }

    private void FireArrow()
    {
        if (_timeToNextShot != 0 || _chargeProgressSeconds < _minProgressToShotSeconds)
        {
            ChangeChargeAnimationState();
            _chargeProgressSeconds = 0;
            return;
        }

        GameObject projectile = Instantiate(_projectile, _firePosition.position, _firePosition.rotation);
        var projectileComponent = projectile.GetComponent<Projectile>();

        projectileComponent.Speed *= _chargeProgressSeconds / _maxChargeSeconds;
        projectileComponent.Direction = _firePosition.right;

        NetworkServer.Spawn(projectile);

        ChangeChargeAnimationState();
        _chargeProgressSeconds = 0;
        _timeToNextShot = _cooldownSeconds;
    }

    private void ChangeChargeAnimationState()
    {
        _isCharging = !_isCharging;
        _animator.SetBool("isCharging", _isCharging);
    }

    public void Attack()
    {
        return;
    }

    public void Hold()
    {
        HandleCharge();
    }

    public void KeyUp()
    {
        FireArrow();
    }

    public void Interact()
    {
        Attack();
    }

    public void FinishHold()
    {
        KeyUp();
    }
}
