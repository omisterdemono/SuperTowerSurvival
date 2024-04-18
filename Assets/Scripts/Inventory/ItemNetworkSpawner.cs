using System;
using System.Linq;
using Inventory.Models;
using Mirror;
using UnityEngine;

namespace Inventory
{
    public class ItemNetworkSpawner : NetworkBehaviour
    {
        [SerializeField] private ItemInWorld _itemPrefab;
        [SerializeField] private ItemDatabaseSO _itemDatabase;

        [Command(requiresAuthority = false)]
        public void SpawnItemCmd(string itemId, int count, Vector3 position)
        {
            var itemInWorld = Instantiate(_itemPrefab, position, Quaternion.identity);
            NetworkServer.Spawn(itemInWorld.gameObject, ownerConnection: null);

            itemInWorld.ItemId = itemId;
            itemInWorld.Count = count;
        }
    }
}