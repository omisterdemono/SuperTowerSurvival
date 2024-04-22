using Assets.Scripts.Weapons;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Instrument : MonoBehaviour, IInstrument, IEquipable
{
    [FormerlySerializedAs("_instrumentAttributes")] [SerializeField] private InstrumentItemSO _instrumentItemSo;
    [SerializeField] private float _cooldownSeconds;
    [SerializeField] private float _obtainSeconds;

    public float Strength { get; set; }
    public float Durability { get; set; }
    public InstrumentType InstrumentType { get; set; }
    public bool NeedRotation { get; set; } = true;
    public bool NeedFlip { get; set; } = true;
    public bool CanPerform => _cooldownComponent.CanPerform;

    public float CooldownSeconds => _cooldownSeconds;

    public Vector3 MousePosition { get; set; }

    private Animator _animator;
    private CooldownComponent _cooldownComponent;
    private Collider2D _obtainColliderTrigger;

    private bool _isObtaining;

    private void Awake()
    {
        Strength = _instrumentItemSo.Strength;
        Durability = _instrumentItemSo.Durability;
        InstrumentType = _instrumentItemSo.InstrumentType;

        _animator = GetComponent<Animator>();
        _cooldownComponent = new CooldownComponent() { CooldownSeconds = _cooldownSeconds };

        _obtainColliderTrigger = GetComponent<Collider2D>();
        _obtainColliderTrigger.enabled = false;
    }

    private void Update()
    {
        _cooldownComponent.HandleCooldown();
    }

    //todo fix
    public void StartObtain()
    {
        _obtainColliderTrigger.enabled = true;
    }

    public void ChangeAnimationState()
    {
        _isObtaining = !_isObtaining;
        _animator.SetBool("isObtaining", _isObtaining);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is not BoxCollider2D)
        {
            return;
        }
        
        if (collision.TryGetComponent<Obtainable>(out var obtainable))
        {
            Obtain(obtainable);
        }
    }

    private void Obtain(Obtainable obtainable)
    {
        obtainable.GetObtained(this);
        Durability -= 1.0f;
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
        yield return new WaitForSeconds(_cooldownSeconds - _obtainSeconds);
        StartObtain();
        yield return new WaitForSeconds(_obtainSeconds);
        FinishObtain();
    }
    
    public void Obtain()
    {
    }

    private void FinishObtain()
    {
        _obtainColliderTrigger.enabled = false;
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
