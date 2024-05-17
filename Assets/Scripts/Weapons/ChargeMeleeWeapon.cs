using Assets.Scripts.Weapons;
using System;
using System.Collections;
using Components;
using TMPro;
using UnityEngine;

public class ChargeMeleeWeapon : MonoBehaviour, IWeapon, IEquipable
{
    [Header("Hit parameters")]
    [SerializeField] private string[] _desiredTargets;
    [SerializeField] private float _damage = 1.0f;
    [SerializeField] private float _minProgressTohitSeconds;
    [SerializeField] private float _maxChargeSeconds;
    [SerializeField] private float _cooldownSeconds;

    //death rotate is name of one rotate that is performed during charge attack
    [SerializeField] private float _countOfDeathRotates;
    [SerializeField] private float _oneDeathRotateStep;
    [SerializeField] private float _oneDeathRotateSeconds;

    public float Damage { get => _damage; set => _damage = value; }
    public bool NeedFlip { get; set; }
    public bool NeedRotation { get; set; } = true;
    public bool CanPerform => _cooldownComponent.CanPerform;
    public float CooldownSeconds => _cooldownSeconds;
    public Vector3 MousePosition { get; set; }

    public static event EventHandler OnMeleeSwing;
    public static event EventHandler OnMeleeSpin;


    private CooldownComponent _cooldownComponent;
    private ChargeComponent _chargeComponent;

    private Collider2D _hitCollider;
    private Vector3 _rotationBeforeCharge;
    private bool _isAttacking;

    private void Awake()
    {
        _cooldownComponent = new CooldownComponent() { CooldownSeconds= _cooldownSeconds };
        _chargeComponent = new ChargeComponent()
        {
            MinProgressToShotSeconds = _minProgressTohitSeconds,
            MaxChargeSeconds = _maxChargeSeconds
        };

        _hitCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        _cooldownComponent.HandleCooldown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //hitting all desired targets
        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if (component != null)
            {
                var knockback = component.GetComponent<KnockbackComponent>();
                Vector2 direction = component.transform.position - this.transform.position;
                knockback?.PlayKnockback(direction.normalized, 10f, 0.05f);
                component.GetComponent<HealthComponent>().Damage(Damage);
                return;
            }
        }
    }

    public void Attack()
    {
        return;
    }

    public void Hold()
    {
        if (_isAttacking || !_cooldownComponent.CanPerform)
        {
            return;
        }

        PrepareForHit();
        HandleCharge();
    }

    private void HandleCharge()
    {
        if (_chargeComponent.MaxCharged)
        {
            return;
        }

        var chargeProgressAngle = _chargeComponent.ChargeProgress * 180.0f;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, chargeProgressAngle));

        _chargeComponent.HandleCharge();
    }

    public void KeyUp()
    {
        ReleaseCharge();
    }

    private void ReleaseCharge()
    {
        if (!_cooldownComponent.CanPerform)
        {
            RecoverAfterHit();
            return;
        }

        if (_chargeComponent.ChargeProgress < 1.0f && _chargeComponent.CanShoot)
        {
            StartCoroutine(AttackRotate());
            OnMeleeSwing?.Invoke(this, EventArgs.Empty);
            return;
        }

        StartCoroutine(DeathRotate());
        OnMeleeSpin?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator AttackRotate()
    {
        _hitCollider.enabled = true;
        _isAttacking = true;

        var angleDiff = _rotationBeforeCharge.z;

        for (float i = 0; i < _oneDeathRotateSeconds; i += _oneDeathRotateStep)
        {
            var rotationStep = transform.localEulerAngles.z - (_oneDeathRotateStep / _oneDeathRotateSeconds * angleDiff);
            transform.localRotation = Quaternion.Euler(0, 0, rotationStep);
            yield return new WaitForSeconds(_oneDeathRotateStep);
        }

        RecoverAfterHit();
    }

    private IEnumerator DeathRotate()
    {
        _hitCollider.enabled = true;
        _isAttacking = true;

        for (float i = 0; i < _countOfDeathRotates * _oneDeathRotateSeconds * _chargeComponent.ChargeProgress; i += _oneDeathRotateStep)
        {
            var rotationStep = transform.localEulerAngles.z - (_oneDeathRotateStep / _oneDeathRotateSeconds * 360.0f);
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotationStep));
            yield return new WaitForSeconds(_oneDeathRotateStep);
        }

        RecoverAfterHit();
    }

    private void PrepareForHit()
    {
        NeedRotation = false;
        NeedFlip = false;
        _rotationBeforeCharge = transform.localEulerAngles;
        _hitCollider.enabled = false;
    }

    private void RecoverAfterHit()
    {
        _chargeComponent.ResetCharge();
        _cooldownComponent.ResetCooldown();

        NeedFlip = true;
        NeedRotation = true;
        _isAttacking = false;

        _hitCollider.enabled = false;

        HandleCharge();
    }

    public void Interact()
    {
        Attack();
    }

    public void FinishHold()
    {
        KeyUp();
    }

    public void ChangeAnimationState()
    {
        return;
    }
}
