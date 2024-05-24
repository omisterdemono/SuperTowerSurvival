using Inventory;
using Inventory.Models;
using Mirror;
using UnityEngine;

namespace Components
{
    public class LootDropper : NetworkBehaviour
    {
        [SerializeField] private ItemSO[] _loot;
        [SerializeField] private int[] _quantity;

        private void Start()
        {
            GetComponent<HealthComponent>().OnDeath += DropItems;
        }

        private void DropItems()
        {
            var itemSpawner = FindObjectOfType<ItemNetworkSpawner>();

            for (int i = 0; i < _loot.Length; i++)
            {
                itemSpawner.SpawnItemCmd(_loot[i].Id, _quantity[i], transform.position);
            }
        }
    }
}