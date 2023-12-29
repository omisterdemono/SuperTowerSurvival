using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class MovingItemUI : MonoBehaviour
    {
        public InventoryCellUI PreviousUICell { get; set; }
        private Image _image;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _image = GetComponentInChildren<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Init(InventoryCellUI inventoryCellUI)
        {
            PreviousUICell = inventoryCellUI;
            
            _image.raycastTarget = false;
            (_image.sprite, _text.text) = inventoryCellUI.ItemUI.CloneForMoving();
        }

        public void Set(Sprite sprite, string countText)
        {
            _image.sprite = sprite;
            _text.text = countText;
        }
        
        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void SetCount(int count)
        {
            _text.text = count.ToString();
        }

        void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}