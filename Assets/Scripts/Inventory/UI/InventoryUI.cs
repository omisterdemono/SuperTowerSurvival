using System.Collections.Generic;
using Infrastructure.Config;
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
        private Character _character;

        private void Awake()
        {
            GetComponentInParent<Canvas>();

            for (int i = 0; i < ConfigConstants.CellsInInventoryCount; i++)
            {
                var inventoryCell = Instantiate(_inventoryCellPrefab, transform);

                inventoryCell.Index = i;
                inventoryCell.ItemMove += OnItemMoveOrCombine;
                inventoryCell.ItemDivide += OnItemDivideOrAddToStack;
                inventoryCell.ItemUse += OnItemUse;

                _uiCells.Add(inventoryCell);
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

            usableItem.PerformAction(_character);
            _inventory.TryRemoveFromCell(currentInventoryUICell.InventoryCell, 1);
        }

        private void SwapItemsInMovingAndCurrent(InventoryCellUI currentInventoryUICell)
        {
            var (item, count) = (_movingItemUI.Item, _movingItemUI.TakenCountOfItems);
            _movingItemUI.Init(currentInventoryUICell.InventoryCell.Item, currentInventoryUICell.InventoryCell.Count);
            (currentInventoryUICell.InventoryCell.Item, currentInventoryUICell.InventoryCell.Count) = (item, count);
        }

        private void AddItemsToCell(InventoryCellUI currentInventoryUICell, int countToAddToCurrent)
        {
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
            _character = playerInventory.GetComponent<Character>();
            _inventory = playerInventory.Inventory;

            for (var index = 0; index < _inventory.Cells.Length; index++)
            {
                var cell = _inventory.Cells[index];
                cell.Modified += _uiCells[index].OnModified;

                _uiCells[index].InventoryCell = cell;
                _uiCells[index].ItemDrop = playerInventory.OnItemDrop;
            }
        }
    }
}