using UnityEngine;

public class Instrument : MonoBehaviour, IInstrument
{
    [SerializeField] private InstrumentAttributes _instrumentAttributes;

    public float Strength { get; set; }
    public float Durability { get; set; }
    public InstrumentType InstrumentType { get; set; }
    
    private Collectable _lastCollectable;

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
        var collectable = collision.GetComponent<Collectable>();
        if (collectable)
        {
            _lastCollectable = collectable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var collectable = collision.GetComponent<Collectable>();
        if (collectable)
        {
            _lastCollectable = null;
        }
    }
}
