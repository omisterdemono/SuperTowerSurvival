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

        public void OnLeftButtonPressed(InventoryCellUI currentInventoryCellUI)
        {
            //skipping blank cells
            if (_movingItemUI == null && !currentInventoryCellUI.Item.IsAssigned)
            {
                return;
            }

            //creating moving item
            if (_movingItemUI == null)
            {
                _movingItemUI = Instantiate(_movingItemUIPrefab, Input.mousePosition, Quaternion.identity, transform);
                _movingItemUI.Init(currentInventoryCellUI);
                
                //todo rework, so hiding will be inside item
                currentInventoryCellUI.Item.gameObject.SetActive(false);
                return;
            }

            //same cell move
            if (_movingItemUI != null && _movingItemUI.PreviousCell == currentInventoryCellUI)
            {
                _movingItemUI.PreviousCell.Item.gameObject.SetActive(true);
                
                Destroy(_movingItemUI.gameObject);
                _movingItemUI = null;
                return;
            }

            //moving item to other cell
            if (_movingItemUI != null)
            {
                _movingItemUI.PreviousCell.Item.gameObject.SetActive(true);
                _inventory.MoveItem(_movingItemUI.PreviousCell.Index, currentInventoryCellUI.Index);
                
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
            }
        }
    }
}