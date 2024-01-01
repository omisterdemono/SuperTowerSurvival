using System;
using Infrastructure;
using Infrastructure.Config;
using Inventory.Models;
using Inventory.UI;
using UnityEngine;

namespace Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private ItemInWorld _itemPrefab;
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
            var itemInWorld = Instantiate(_itemPrefab, transform.position + direction, Quaternion.identity);
            
            itemInWorld.Item = inventoryCell.Item;
            itemInWorld.Count = count;
            
            Inventory.TryRemoveFromCell(inventoryCell, count);
        }
    }
}
