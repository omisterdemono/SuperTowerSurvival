using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class MovingItemUI : MonoBehaviour
    {
        public InventoryCellUI PreviousCell { get; set; }
        private Image _image;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _image = GetComponentInChildren<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Init(InventoryCellUI inventoryCellUI)
        {
            PreviousCell = inventoryCellUI;
            
            _image.raycastTarget = false;
            (_image.sprite, _text.text) = inventoryCellUI.Item.CloneForMoving();
        }

        void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}