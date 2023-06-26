using Assets.Scripts.Weapons;
using System;
using System.Collections;
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
    public bool CanPerform => throw new NotImplementedException();

    public float CooldownSeconds => throw new NotImplementedException();

    public Vector3 MousePosition { get; set; }

    private Collider2D _hitCollider;
    private Vector3 _rotationBeforeCharge;
    private float _chargeProgressSeconds;
    private float _timeToNextHit;
    private bool _isAttacking;

    private void Awake()
    {
        _hitCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        HandleCooldown();
    }

    private void HandleCooldown()
    {
        if (_timeToNextHit == 0)
        {
            return;
        }

        if (_timeToNextHit < 0)
        {
            _timeToNextHit = 0;
            return;
        }

        _timeToNextHit -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //hitting all desired targets
        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if (component != null)
            {
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
        if (_isAttacking || _timeToNextHit != 0)
        {
            return;
        }

        PrepareForHit();

        HandleCharge();
    }

    private void HandleCharge()
    {
        if (_chargeProgressSeconds == _maxChargeSeconds)
        {
            return;
        }

        if (_chargeProgressSeconds > _maxChargeSeconds)
        {
            _chargeProgressSeconds = _maxChargeSeconds;
        }

        var chargeProgressAngle = _chargeProgressSeconds / _maxChargeSeconds * 180.0f;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, chargeProgressAngle));

        _chargeProgressSeconds += Time.deltaTime;
    }

    public void KeyUp()
    {
        ReleaseCharge();
    }

    private void ReleaseCharge()
    {
        if (_timeToNextHit != 0)
        {
            RecoverAfterHit();
            return;
        }

        if (_chargeProgressSeconds < _maxChargeSeconds && _chargeProgressSeconds >= _minProgressTohitSeconds)
        {
            StartCoroutine(AttackRotate());
            return;
        }

        StartCoroutine(DeathRotate());
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

        var chargeProgress = _chargeProgressSeconds / _maxChargeSeconds;
        for (float i = 0; i < _countOfDeathRotates * _oneDeathRotateSeconds * chargeProgress; i += _oneDeathRotateStep)
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
        _chargeProgressSeconds = 0;
        _timeToNextHit = _cooldownSeconds;

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
