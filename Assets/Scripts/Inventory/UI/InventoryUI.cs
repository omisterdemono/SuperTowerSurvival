using System;
using Inventory.Model;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Inventory.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        [SerializeField] private InventoryCellUI _inventoryCellPrefab;
        [SerializeField] private MovingItemUI _movingItemUIPrefab;

        private List<InventoryCellUI> _cells = new();

        private MovingItemUI _movingItemUI;
        private Inventory _inventory;

        private void Awake()
        {
            GetComponentInParent<Canvas>();

            for (int i = 0; i < _rows * _columns; i++)
            {
                var inventoryCell = Instantiate(_inventoryCellPrefab, transform);
                inventoryCell.Index = i;
                inventoryCell.OnLeftButtonPressed += OnLeftButtonPressed;

                _cells.Add(inventoryCell);
            }
        }

        public void OnLeftButtonPressed(InventoryCellUI currentInventoryUICell)
        {
            //skipping blank cells
            if (_movingItemUI == null && !currentInventoryUICell.ItemUI.IsAssigned)
            {
                return;
            }

            //creating moving item
            if (_movingItemUI == null)
            {
                _movingItemUI = Instantiate(_movingItemUIPrefab, Input.mousePosition, Quaternion.identity, transform);
                _movingItemUI.Init(currentInventoryUICell);
                
                //todo rework, so hiding will be inside item
                currentInventoryUICell.ItemUI.gameObject.SetActive(false);
                return;
            }

            //same cell move
            if (_movingItemUI != null && _movingItemUI.PreviousUICell == currentInventoryUICell)
            {
                _movingItemUI.PreviousUICell.ItemUI.gameObject.SetActive(true);
                
                Destroy(_movingItemUI.gameObject);
                _movingItemUI = null;
                return;
            }
            
            //combining stacks
            if (_movingItemUI.PreviousUICell.InventoryCell.Item == currentInventoryUICell.InventoryCell.Item)
            {
                var countInPrevious = _movingItemUI.PreviousUICell.InventoryCell.Count;
                var availableInCurrent = currentInventoryUICell.InventoryCell.AvailableCount;

                if (availableInCurrent != 0)
                {
                    var left = _inventory.TryAddToCell(currentInventoryUICell.Index, countInPrevious);
                    _inventory.TryRemoveFromCell(_movingItemUI.PreviousUICell.Index, availableInCurrent);
                    
                    _movingItemUI.SetCount(left);
                    
                    return;
                }
            }
            
            //swapping items in moving and current
            if (_movingItemUI != null)
            {
                _inventory.MoveItem(_movingItemUI.PreviousUICell.Index, currentInventoryUICell.Index);

                var (sprite, text) = _movingItemUI.PreviousUICell.ItemUI.CloneForMoving();
                _movingItemUI.Set(sprite, text);
            }
            
            //placing item in free cell
            if (_movingItemUI != null && currentInventoryUICell.InventoryCell.IsFree)
            {
                _movingItemUI.PreviousUICell.ItemUI.gameObject.SetActive(true);
                _inventory.MoveItem(_movingItemUI.PreviousUICell.Index, currentInventoryUICell.Index);

                Destroy(_movingItemUI.gameObject);
                _movingItemUI = null;
            }
        }

        public void AttachInventory(Inventory inventory)
        {
            _inventory = inventory;
            for (var index = 0; index < inventory.Cells.Length; index++)
            {
                var cell = inventory.Cells[index];
                cell.Modified += _cells[index].OnModified;
                _cells[index].InventoryCell = cell;
            }
        }
    }
}