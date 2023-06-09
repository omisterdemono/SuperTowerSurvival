using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private GameObject[] _loot;
    [SerializeField] private HealthComponent _healthComponent;
    [SerializeField] private InstrumentType _instrument;

    private void Awake()
    {
        _healthComponent= GetComponent<HealthComponent>();
    }

    public void GetCollected(IInstrument collectingInstrument)
    {
        if (collectingInstrument.InstrumentType != _instrument)
        {
            return;
        }

        _healthComponent.Damage(collectingInstrument.Strength);
    }

    private void DropLoot()
    {
        throw new NotImplementedException();        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
