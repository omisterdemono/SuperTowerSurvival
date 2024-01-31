using Inventory.Models;
using Mirror;
using System;
using Inventory;
using UnityEditor.Searcher;
using UnityEngine;

public class Obtainable : NetworkBehaviour
{
    [SerializeField] private ItemSO[] _loot;
    [SerializeField] private int[] _quantity;
    [SerializeField] private InstrumentType _instrument;
    
    private HealthComponent _healthComponent;

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

    private void GetDestroyed()
    {
        var itemSpawner = FindObjectOfType<ItemNetworkSpawner>();

        for (int i = 0; i < _loot.Length; i++)
        {
            itemSpawner.SpawnItemCmd(_loot[i].Id, _quantity[i], transform.position);
        }

        NetworkServer.Destroy(this.gameObject);
    }
}
