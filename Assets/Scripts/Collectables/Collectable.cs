using Mirror;
using System;
using UnityEngine;

public class Collectable : NetworkBehaviour
{
    [SerializeField] private GameObject[] _loot;
    [SerializeField] private HealthComponent _healthComponent;
    [SerializeField] private InstrumentType _instrument;

    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();

        _healthComponent.OnDeath += GetDestroyed;
    }

    public void GetObtained(IInstrument collectingInstrument)
    {
        if (collectingInstrument.InstrumentType != _instrument)
        {
            return;
        }

        _healthComponent.Damage(collectingInstrument.Strength);
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= GetDestroyed;
    }

    [Command(requiresAuthority = false)]
    private void GetDestroyed()
    {
        //dropping items
        foreach (var drop in _loot)
        {
            NetworkServer.Spawn(Instantiate(drop, transform));
        }

        //deleting on server side
        NetworkServer.Destroy(this.gameObject);
    }
}
