using Inventory.Model;
using Mirror;
using System;
using UnityEngine;

public class Obtainable : NetworkBehaviour
{
    [SerializeField] private ItemSO[] _loot;
    [SerializeField] private int[] _quantity;
    [SerializeField] private InstrumentType _instrument;
    
    private HealthComponent _healthComponent;
    //public InventorySO LastInventory { get; set; }

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

    //todo implement
    private void GetDestroyed()
    {
        throw new NotImplementedException();

        //for (int i = 0; i < _loot.Length; i++)
        //{
        //    LastInventory.AddItem(_loot[i], _quantity[i]);
        //}

        //NetworkServer.Destroy(this.gameObject);
    }
}
