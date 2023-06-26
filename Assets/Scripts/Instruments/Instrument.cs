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

    private Obtainable _lastObtainable;

    private void Awake()
    {
        Strength = _instrumentAttributes.Strength;
        Durability = _instrumentAttributes.Durability;
        InstrumentType = _instrumentAttributes.InstrumentType;
    }

    public void Obtain()
    {
        if (!_lastObtainable)
        {
            return;
        }

        _lastObtainable.GetObtained(this);
        Durability -= 1.0f;
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
}
