using System;
using Config;
using Infrastructure;
using Inventory.Models;
using Inventory.UI;
using Mirror;
using UnityEngine;

namespace Inventory
{
    public class PlayerInventory : NetworkBehaviour
    {
        [SerializeField] private float _throwRadius;
        public Inventory Inventory { get; private set; }
        public Character Character { get; private set; }
        public CraftingSystem CraftingSystem { get; private set; }
        public Vector3 LastMoveDirection { get; set; }

        private InventoryUI _inventoryUI;
        private CraftingUI _craftingUI;
        private ItemNetworkSpawner _itemNetworkSpawner;

        private GameObject _inventoryHolderUI;
        public bool IsInventoryShown { get; private set; } = true;

        private void Start()
        {
            if (!isOwned) return;
            InitInventoryAndUI();
        }

        private void InitInventoryAndUI()
        {
            Inventory = new Inventory(GameConfig.InventoryCellsCount + GameConfig.HotbarCellsCount +
                                      GameConfig.EquipCellsCount);
            CraftingSystem = new CraftingSystem(Inventory);

            Character = GetComponent<Character>();

            _inventoryHolderUI = GameObject.FindWithTag("InventoryHolderUI");

            var gameInitializer = FindObjectOfType<GameInitializer>();
            _inventoryUI = gameInitializer.InitializeInventoryUI();
            _inventoryUI.AttachInventory(this);
            ChangeInventoryUIState();

            _craftingUI = gameInitializer.InitializeCraftingUI();
            _craftingUI.AttachInventory(this);

            _itemNetworkSpawner = FindObjectOfType<ItemNetworkSpawner>();
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
            _itemNetworkSpawner.SpawnItemCmd(inventoryCell.Item.Id, count, transform.position + direction);

            Inventory.TryRemoveFromCell(inventoryCell, count);
        }

        public void ChangeInventoryUIState()
        {
            IsInventoryShown = !IsInventoryShown;
            _inventoryHolderUI.gameObject.SetActive(IsInventoryShown);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!isOwned) return;
            if (col.TryGetComponent<ItemInWorld>(out var item))
            {
                Inventory.TryAddItem(item.Item, item.Count);
                item.GetPickedUp();
            }
        }
    }
}