using Assets.Scripts.Weapons;
using UnityEngine;

public class Instrument : MonoBehaviour, IInstrument, IEquipable
{
    [SerializeField] private InstrumentAttributes _instrumentAttributes;

    public float Strength { get; set; }
    public float Durability { get; set; }
    public InstrumentType InstrumentType { get; set; }
    public bool NeedFlip { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool NeedRotation { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private Obtainable _lastCollectable;

    private void Awake()
    {
        Strength = _instrumentAttributes.Strength;
        Durability = _instrumentAttributes.Durability;
        InstrumentType = _instrumentAttributes.InstrumentType;
    }

    public void Obtain()
    {
        if (!_lastCollectable)
        {
            return;
        }

        _lastCollectable.GetObtained(this);
        Durability -= 1.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collectable = collision.GetComponent<Obtainable>();
        if (collectable)
        {
            _lastCollectable = collectable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var collectable = collision.GetComponent<Obtainable>();
        if (collectable)
        {
            _lastCollectable = null;
        }
    }
}
