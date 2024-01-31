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

            var item = _itemDatabase.Items.FirstOrDefault(i => i.Id == itemId) ?? throw new ArgumentNullException("itemId is incorrect");
            itemInWorld.Item = item;
            itemInWorld.Count = count;
        }
    }
}