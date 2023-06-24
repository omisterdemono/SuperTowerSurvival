using Assets.Scripts.Weapons;
using UnityEngine;

public class Instrument : MonoBehaviour, IInstrument, IEquipable
{
    [SerializeField] private InstrumentAttributes _instrumentAttributes;
    [SerializeField] private float _cooldownSeconds;

    public float Strength { get; set; }
    public float Durability { get; set; }
    public InstrumentType InstrumentType { get; set; }
    public bool NeedFlip { get; set; }
    public bool NeedRotation { get; set; }

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

    public void Obtain()
    {
        if (!_lastObtainable)
        {
            return;
        }

        _lastObtainable.GetObtained(this);
        Durability -= 1.0f;

        ChangeObtainingState();
    }

    public void ChangeObtainingState()
    {
        _isObtaining = !_isObtaining;
        _animator.SetBool("isObtaining", _isObtaining);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obtainable = collision.GetComponent<Obtainable>();
        if (obtainable)
        {
            _lastObtainable = obtainable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var collectable = collision.GetComponent<Obtainable>();
        if (collectable)
        {
            _lastObtainable = null;
        }
    }

    public void Interact()
    {
        if (!_cooldownComponent.CanHit)
        {
            return;
        }
        _cooldownComponent.ResetCooldown();

        ChangeObtainingState();
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
