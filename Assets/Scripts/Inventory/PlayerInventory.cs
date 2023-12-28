using System;
using Infrastructure;
using Inventory.UI;
using UnityEngine;

namespace Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int _count = 18;
        
        private Inventory _inventory;
        private InventoryUI _inventoryUI;
        public Inventory Inventory => _inventory;

        private void Awake()
        {
            _inventory = new Inventory(_count);
            
            _inventoryUI = FindObjectOfType<GameInitializer>().InitializeInventoryUI();
            _inventoryUI.AttachInventory(_inventory);
        }
    }
}
