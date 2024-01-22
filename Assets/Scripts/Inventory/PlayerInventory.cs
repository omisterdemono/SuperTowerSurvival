using System;
using System.Linq;
using Infrastructure;
using Infrastructure.Config;
using Inventory.Models;
using Inventory.UI;
using Mirror;
using UnityEngine;

namespace Inventory
{
    public class PlayerInventory : NetworkBehaviour
    {
        [SerializeField] private ItemInWorld _itemPrefab;
        [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private float _throwRadius;
        public Inventory Inventory { get; private set; }
        public Vector3 LastMoveDirection { get; set; }

        private InventoryUI _inventoryUI;

        private void Awake()
        {
            Inventory = new Inventory(ConfigConstants.CellsInInventoryCount);
            
            _inventoryUI = FindObjectOfType<GameInitializer>().InitializeInventoryUI();
            _inventoryUI.AttachInventory(this);
        }

        public void OnItemDrop(InventoryCell inventoryCell, int count)
        {
            if (inventoryCell.IsFree)
            {
                return;
            }
            
            var direction = LastMoveDirection;
            direction.x += _throwRadius;
            direction.y += _throwRadius;
            SpawnItem(inventoryCell.Item.Id, count, direction);

            Inventory.TryRemoveFromCell(inventoryCell, count);
        }

        [Command]
        private void SpawnItem(string itemId, int count, Vector3 direction)
        {
            var itemInWorld = Instantiate(_itemPrefab, transform.position + direction, Quaternion.identity);

             var item = _itemDatabase.Items.FirstOrDefault(i => i.Id == itemId) ?? throw new ArgumentNullException("itemId is incorrect");
             itemInWorld.Item = item;
             itemInWorld.Count = count;
        }
    }
}
