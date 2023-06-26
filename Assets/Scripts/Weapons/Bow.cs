using Assets.Scripts.Weapons;
using Mirror;
using System;
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
    private CooldownComponent _cooldownComponent;
    private ChargeComponent _chargeComponent;

    public float Damage { get => _damage; set => _damage = value; }
    public bool NeedFlip { get => _needFlip; set => _needFlip = value; }
    public bool NeedRotation { get; set; } = true;
    public bool CanPerform => _cooldownComponent.CanPerform;
    public float CooldownSeconds => _cooldownSeconds;

    public Vector3 MousePosition { get; set; }

    private bool _isCharging = false;
    private float _chargeProgressSeconds;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _cooldownComponent = new CooldownComponent() { CooldownSeconds = _cooldownSeconds };
        _chargeComponent = new ChargeComponent() 
        { 
            MaxChargeSeconds = _maxChargeSeconds,
            MinProgressToShotSeconds= _minProgressToShotSeconds 
        };
    }

    private void Update()
    {
        _cooldownComponent.HandleCooldown();
    }

    private void FireArrow()
    {
        if (!_cooldownComponent.CanPerform || !_chargeComponent.CanShoot)
        {
            ChangeChargeAnimationState();
            _chargeComponent.CancelCharge();
            return;
        }

        GameObject projectile = Instantiate(_projectile, _firePosition.position, _firePosition.rotation);
        var projectileComponent = projectile.GetComponent<Projectile>();

        projectileComponent.Speed *= _chargeComponent.ChargeProgress;
        projectileComponent.Direction = _firePosition.right;

        NetworkServer.Spawn(projectile);

        ChangeChargeAnimationState();
        _chargeComponent.CancelCharge();
        _cooldownComponent.ResetCooldown();
    }

    public void Attack()
    {
        if (_chargeProgressSeconds >= _maxChargeSeconds)
        {
            FireArrow(direction);
        }
        else
        {
            Hold(direction);
        }
        //return;
    }

    public void Hold()
    {
        if (!_chargeComponent.IsCharging)
        {
            ChangeChargeAnimationState();
        }

        _chargeComponent.HandleCharge();
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

    public void ChangeChargeAnimationState()
    {
        _isCharging = !_isCharging;
        _animator.SetBool("isCharging", _isCharging);
    }

    public void ChangeAnimationState()
    {
        return;
    }

    public float GetChargeProgressSeconds()
    {
        return _chargeProgressSeconds;
    }
    public float GetMaxChargeSeconds()
    {
        return _maxChargeSeconds;
    }
}
