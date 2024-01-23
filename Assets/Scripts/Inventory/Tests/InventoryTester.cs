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

        private PlayerInventory PlayerInventory
        {
            get
            {
                if (_playerInventory == null)
                {
                    _playerInventory = FindObjectOfType<PlayerInventory>();
                }
                return _playerInventory;
            }
            set => _playerInventory = value;
        }

        [Header("Testing")]
        [SerializeField] private ItemSO[] _items;
        [SerializeField] private int _itemCount;

        public void AddItem(int index)
        {
            PlayerInventory.LastMoveDirection = new Vector3(1.0f, 1.0f);
            
            PlayerInventory.Inventory.TryAddItem(_items[index], _itemCount);
        }
        
        public void RemoveItem(int index)
        {
            PlayerInventory.Inventory.TryRemoveItem(_items[index], _itemCount);
        }
    }
}