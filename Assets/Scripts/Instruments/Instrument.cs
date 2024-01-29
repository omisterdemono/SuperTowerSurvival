using Assets.Scripts.Weapons;
using System;
using System.Collections;
using UnityEngine;

public class Instrument : MonoBehaviour, IInstrument, IEquipable
{
    [SerializeField] private InstrumentAttributes _instrumentAttributes;
    [SerializeField] private float _cooldownSeconds;

    public float Strength { get; set; }
    public float Durability { get; set; }
    public InstrumentType InstrumentType { get; set; }
    public bool NeedRotation { get; set; } = true;
    public bool NeedFlip { get; set; } = true;
    public bool CanPerform => _cooldownComponent.CanPerform;

    public float CooldownSeconds => _cooldownSeconds;

    public Vector3 MousePosition { get; set; }

    private Obtainable _lastObtainable;
    private Animator _animator;
    private CooldownComponent _cooldownComponent;

    private bool _isObtaining;

    private void Awake()
    {
        Strength = _instrumentAttributes.Strength;
        Durability = _instrumentAttributes.Durability;
        InstrumentType = _instrumentAttributes.InstrumentType;

        _animator = GetComponent<Animator>();
        _cooldownComponent = new CooldownComponent() { CooldownSeconds = _cooldownSeconds };
    }

    private void Update()
    {
        _cooldownComponent.HandleCooldown();
    }

    //todo fix
    public void Obtain()
    {
        // throw new NotImplementedException();

        if (!_lastObtainable)
        {
            return;
        }

        _lastObtainable.GetObtained(this);
        Durability -= 1.0f;
    }

    public void ChangeAnimationState()
    {
        _isObtaining = !_isObtaining;
        _animator.SetBool("isObtaining", _isObtaining);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is BoxCollider2D)
        {
            
        }
        
        if (!collision.TryGetComponent<Obtainable>(out var obtainable) || obtainable == _lastObtainable)
        {
            return;
        }

        if (obtainable)
        {
            _lastObtainable = obtainable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision is BoxCollider2D)
        {
            
        }
        
        var obtainable = collision.GetComponent<Obtainable>();

        if (obtainable != _lastObtainable)
        {
            return;
        }

        if (obtainable)
        {
            _lastObtainable = null;
        }
    }

    public void Interact()
    {
        if (!_cooldownComponent.CanPerform)
        {
            return;
        }
        _cooldownComponent.ResetCooldown();

        StartCoroutine(DelayedObtain());
    }

    private IEnumerator DelayedObtain()
    {
        yield return new WaitForSeconds(_cooldownSeconds);
        Obtain();
    }

    public void Hold()
    {
        Interact();
    }

    public void FinishHold()
    {
        return;
    }
}
