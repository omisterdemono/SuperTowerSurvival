using System;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class InventoryCellUI : MonoBehaviour, IPointerClickHandler
    {
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
                    //logic for dividing
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    break;
            }
            //Item.transform.SetParent(canvas.transform, false);
            //Item.transform.SetAsLastSibling();
        }
    }
}