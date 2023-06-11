using UnityEngine;

public class Pickaxe : MonoBehaviour, IInstrument
{
    [SerializeField] private float _strength;
    [SerializeField] private float _durability;
    [SerializeField] private InstrumentType _instrumentType;
    public float Strength { get => _strength; set => _strength = value; }
    public float Durability { get => _durability; set => _durability = value; }
    public InstrumentType InstrumentType { get => _instrumentType; set => _instrumentType = value; }

    private Collectable _lastCollectable;

    public void Obtain()
    {
        if (!_lastCollectable)
        {
            return;
        }

        _lastCollectable.GetObtained(this);
        _durability -= 1.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collectable = collision.GetComponent<Collectable>();
        if (collectable)
        {
            _lastCollectable= collectable;
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
