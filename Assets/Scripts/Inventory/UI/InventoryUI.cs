using Inventory.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        [SerializeField] private InventoryCellUI _inventoryCellPrefab;

        [Header("Testing")]
        [SerializeField] private InventoryItemUI _inventoryItemPrefab;
        [SerializeField] private ItemSO _inventoryItemPrefs;

        private List<InventoryCellUI> _inventoryCells = new();
        private Canvas _canvas;
        private InventoryCellUI _firstInventoryCell;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            for (int i = 0; i < _rows * _columns; i++)
            {
                var inventoryCell = Instantiate(_inventoryCellPrefab, transform);
                inventoryCell.OnLeftButtonPressed += OnLeftButtonPressed;

                _inventoryCells.Add(inventoryCell);
            }
        }

        public void AddItemToFirstSlot()
        {
            _inventoryCells[0].Item = Instantiate(_inventoryItemPrefab, _inventoryCells[0].transform);
            _inventoryCells[0].Item.SetItem(_inventoryItemPrefs, 10);
        }

        public void OnLeftButtonPressed(InventoryCellUI currentInventoryCellUI)
        {
            //its first and item is empty
            if (_firstInventoryCell == null && currentInventoryCellUI.Item == null)
            {
                return;
            }

            //its first and item is not empty
            if (_firstInventoryCell == null)
            {
                _firstInventoryCell = currentInventoryCellUI;
                _firstInventoryCell.Item.SetIsGettingMoved(_canvas.transform);
                return;
            }

            //its first again
            if (_firstInventoryCell != null && _firstInventoryCell == currentInventoryCellUI)
            {
                _firstInventoryCell.Item.SetIsGettingMoved(_firstInventoryCell.transform);
                _firstInventoryCell = null;
                return;
            }

            //its second and is is free
            if (_firstInventoryCell != null && currentInventoryCellUI.Item == null)
            {
                currentInventoryCellUI.Item = _firstInventoryCell.Item;
                currentInventoryCellUI.Item.SetIsGettingMoved(currentInventoryCellUI.transform);

                _firstInventoryCell.Item = null;
                _firstInventoryCell = null;
                return;
            }

            //its second and is not free
            if (_firstInventoryCell != null && currentInventoryCellUI.Item == null)
            {
                var currentItem = _firstInventoryCell.Item;
                currentInventoryCellUI.Item = _firstInventoryCell.Item;
                _firstInventoryCell.Item = currentItem;

                currentInventoryCellUI.Item.SetIsGettingMoved(currentInventoryCellUI.transform);
                _firstInventoryCell.Item.transform.parent = _firstInventoryCell.transform;

                _firstInventoryCell = null;
                return;
            }
        }
    }
}