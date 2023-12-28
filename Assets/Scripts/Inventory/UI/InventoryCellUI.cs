using System;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using Inventory.Model;
using Inventory.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class InventoryCellUI : MonoBehaviour, IPointerClickHandler
    {
        public int Index { get; set; }
        public InventoryItemUI Item { get; set; }
        public Action<InventoryCellUI> OnLeftButtonPressed;

        private void Awake()
        {
            Item = GetComponentInChildren<InventoryItemUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    OnLeftButtonPressed.Invoke(this);
                    break;
                case PointerEventData.InputButton.Right:
                    //logic for dividing or using item
                    break;
            }
        }

        public void OnModified(InventoryCell cell)
        {
            Item.SetItem(cell.Item, cell.Count);
        }

        private void OnDestroy()
        {
            Debug.Log("Destroyed cell");
        }
    }
}