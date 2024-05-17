using System;
using System.Collections.Generic;
using Config;
using Infrastructure;
using Inventory.Models;
using UnityEngine;

namespace Inventory.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventoryCellUI _inventoryCellPrefab;
        [SerializeField] private MovingItemUI _movingItemUIPrefab;

        private List<InventoryCellUI> _uiCells = new();

        private MovingItemUI _movingItemUI;
        private Inventory _inventory;
        private PlayerInventory _playerInventory;
        private HotBar _hotBar;

        private void Awake()
        {
            GetComponentInParent<Canvas>();

            var gameInitializer = FindObjectOfType<GameInitializer>();
            _hotBar = gameInitializer.InitializeHotbar();
            _uiCells.AddRange(_hotBar.HotbarCells);

            var equipUI = gameInitializer.InitializeEquipUI();

            for (int i = 0; i < GameConfig.EquipCellsCount; i++)
            {
                var inventoryCell = Instantiate(_inventoryCellPrefab, equipUI.transform);
                _uiCells.Add(inventoryCell);
            }

            for (int i = 0; i < GameConfig.InventoryCellsCount; i++)
            {
                var inventoryCell = Instantiate(_inventoryCellPrefab, transform);
                _uiCells.Add(inventoryCell);
            }

            for (int i = 0; i < _uiCells.Count; i++)
            {
                var inventoryCell = _uiCells[i];
                inventoryCell.Index = i;
                inventoryCell.ItemMove += OnItemMoveOrCombine;
                inventoryCell.ItemDivide += OnItemDivideOrAddToStack;
                inventoryCell.ItemUse += OnItemUse;
            }
        }

        private void OnItemDivideOrAddToStack(InventoryCellUI currentInventoryUICell)
        {
            //skipping blank cells
            if ((_movingItemUI == null && currentInventoryUICell.InventoryCell.IsFree) ||
                (_movingItemUI == null && !currentInventoryUICell.InventoryCell.IsFree &&
                 currentInventoryUICell.InventoryCell.Count < 2))
            {
                return;
            }

            //creating moving item
            if (_movingItemUI == null)
            {
                CreateMovingItem(currentInventoryUICell, true);
                return;
            }

            //adding one item per click
            if (currentInventoryUICell.InventoryCell.IsFree ||
                (currentInventoryUICell.InventoryCell.Item == _movingItemUI.Item
                 && !currentInventoryUICell.InventoryCell.IsFull))
            {
                AddItemsToCell(currentInventoryUICell, 1);
            }
        }

        private void CreateMovingItem(InventoryCellUI currentInventoryUICell, bool createPartial = false)
        {
            _movingItemUI = Instantiate(_movingItemUIPrefab, Input.mousePosition, Quaternion.identity, transform);
            _movingItemUI.Init(currentInventoryUICell.InventoryCell.Item, currentInventoryUICell.InventoryCell.Count,
                createPartial);
            _inventory.TryRemoveFromCell(currentInventoryUICell.InventoryCell, _movingItemUI.TakenCountOfItems);
        }

        private void OnItemMoveOrCombine(InventoryCellUI currentInventoryUICell)
        {
            //skipping blank cells
            if (_movingItemUI == null && currentInventoryUICell.InventoryCell.IsFree)
            {
                return;
            }

            if (_movingItemUI == null)
            {
                CreateMovingItem(currentInventoryUICell);
                return;
            }

            if (currentInventoryUICell.InventoryCell.IsFree ||
                _movingItemUI.Item == currentInventoryUICell.InventoryCell.Item)
            {
                AddItemsToCell(currentInventoryUICell, _movingItemUI.TakenCountOfItems);
            }
            else
            {
                SwapItemsInMovingAndCurrent(currentInventoryUICell);
            }
        }

        private void OnItemUse(InventoryCellUI currentInventoryUICell)
        {
            var usableItem = currentInventoryUICell.InventoryCell.Item as UsableItemSO;
            if (usableItem == null)
            {
                return;
            }

            usableItem.PerformAction(_playerInventory, () => RemoveItem(currentInventoryUICell));
        }

        private void RemoveItem(InventoryCellUI currentInventoryUICell)
        {
            _inventory.TryRemoveFromCell(currentInventoryUICell.InventoryCell, 1);
            Debug.Log("Removed");
        }

        private void SwapItemsInMovingAndCurrent(InventoryCellUI currentInventoryUICell)
        {
            if (currentInventoryUICell.InventoryCell.IsEquipSlot)
            {
                var armorItem = _movingItemUI.Item as ArmorItemSO;
                if (armorItem == null || currentInventoryUICell.InventoryCell.ArmorType != armorItem.ArmorType)
                {
                    return;
                }
            }
            
            var (item, count) = (_movingItemUI.Item, _movingItemUI.TakenCountOfItems);
            _movingItemUI.Init(currentInventoryUICell.InventoryCell.Item, currentInventoryUICell.InventoryCell.Count);
            (currentInventoryUICell.InventoryCell.Item, currentInventoryUICell.InventoryCell.Count) = (item, count);
        }

        private void AddItemsToCell(InventoryCellUI currentInventoryUICell, int countToAddToCurrent)
        {
            if (currentInventoryUICell.InventoryCell.IsEquipSlot)
            {
                var armorItem = _movingItemUI.Item as ArmorItemSO;
                if (armorItem == null || currentInventoryUICell.InventoryCell.ArmorType != armorItem.ArmorType)
                {
                    return;
                }
            }
            
            var left = _inventory.TryAddToCell(_movingItemUI.Item, currentInventoryUICell.Index,
                countToAddToCurrent);

            _movingItemUI.TakenCountOfItems -= countToAddToCurrent - left;

            if (_movingItemUI.TakenCountOfItems == 0)
            {
                Destroy(_movingItemUI.gameObject);
                _movingItemUI = null;
            }
        }

        public void AttachInventory(PlayerInventory playerInventory)
        {
            _playerInventory = playerInventory;
            _hotBar.PlayerInventory = playerInventory;
            _inventory = playerInventory.Inventory;

            for (var index = 0; index < _inventory.Cells.Length; index++)
            {
                var cell = _inventory.Cells[index];
                cell.Modified += _uiCells[index].OnModified;

                _uiCells[index].InventoryCell = cell;
                _uiCells[index].ItemDrop = playerInventory.OnItemDrop;
            }

            //setting up equip slots
            for (int i = 0; i < GameConfig.EquipCellsCount; i++)
            {
                var inventoryCell = _uiCells[GameConfig.HotbarCellsCount + i];
                inventoryCell.InventoryCell.IsEquipSlot = true;
                inventoryCell.InventoryCell.ArmorType = (ArmorType)(i + 1);
            }
        }
    }
}