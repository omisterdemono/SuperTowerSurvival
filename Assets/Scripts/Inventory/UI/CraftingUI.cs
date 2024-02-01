using System;
using UnityEngine;

namespace Inventory.UI
{
    public class CraftingUI : MonoBehaviour
    {
        [SerializeField] private CraftingDatabaseSO _craftingDatabase;
        [SerializeField] private Transform _tabs;
        [SerializeField] private Transform _itemsHolder;
        [SerializeField] private GameObject _contentPrefab;
        
        private Inventory _inventory;
        private CraftingSystem _craftingSystem;

        public void AttachInventory(PlayerInventory playerInventory)
        {
            _inventory = playerInventory.Inventory;
            _inventory.InventoryChanged += OnInventoryChanged;

            _craftingSystem = playerInventory.CraftingSystem;
        }

        private void OnInventoryChanged()
        {
            throw new NotImplementedException();
        }
    }
}