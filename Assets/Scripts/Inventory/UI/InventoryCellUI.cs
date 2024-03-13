using System;
using Inventory.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class InventoryCellUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject _selectedImage;
        public int Index { get; set; }
        public InventoryItemUI ItemUI { get; set; }
        public InventoryCell InventoryCell { get; set; }
        public Action<InventoryCellUI> ItemMove;
        public Action<InventoryCellUI> ItemDivide;
        public Action<InventoryCellUI> ItemUse;
        public Action<InventoryCell, int> ItemDrop;

        private bool _isHovered = false;

        private void Awake()
        {
            ItemUI = GetComponentInChildren<InventoryItemUI>();
        }

        private void Update()
        {
            if (_isHovered)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
                {
                    ItemDrop?.Invoke(InventoryCell, InventoryCell.Count);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    ItemDrop?.Invoke(InventoryCell, 1);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftControl))
            {
                //dividing items or adding one to stack 
                ItemDivide?.Invoke(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                //moving and combining items
                ItemMove?.Invoke(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                //using items
                ItemUse?.Invoke(this);
            }
        }

        public void OnModified(InventoryCell cell)
        {
            ItemUI.SetItem(cell.Item, cell.Count);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
        }

        public void SetSelect(bool selectType)
        {
            _selectedImage.SetActive(selectType);
        }
    }
}