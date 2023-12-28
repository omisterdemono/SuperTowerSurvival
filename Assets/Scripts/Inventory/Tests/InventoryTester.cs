using System;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory.Tests
{
    public class InventoryTester : MonoBehaviour
    {
        [SerializeField] private PlayerInventory _playerInventory;
        
        [Header("Testing")]
        [SerializeField] private ItemSO _item;
        [SerializeField] private int _itemCount;

        public void AddItem()
        {
            _playerInventory.Inventory.TryAddItem(_item, _itemCount);
        }
        
        public void RemoveItem()
        {
            _playerInventory.Inventory.TryRemoveItem(_item, _itemCount);
        }
    }
}