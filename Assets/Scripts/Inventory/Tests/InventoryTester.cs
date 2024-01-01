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
        [SerializeField] private ItemSO[] _items;
        [SerializeField] private int _itemCount;

        public void AddItem(int index)
        {
            _playerInventory.LastMoveDirection = new Vector3(1.0f, 1.0f);
            
            _playerInventory.Inventory.TryAddItem(_items[index], _itemCount);
        }
        
        public void RemoveItem(int index)
        {
            _playerInventory.Inventory.TryRemoveItem(_items[index], _itemCount);
        }
    }
}