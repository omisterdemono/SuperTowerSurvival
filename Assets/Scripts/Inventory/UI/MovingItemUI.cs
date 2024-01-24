using System;
using Inventory.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class MovingItemUI : MonoBehaviour
    {
        public ItemSO Item
        {
            get => _item;
            set
            {
                _item = value;
                _image.sprite = _item.Sprite;
            }
        }

        public int TakenCountOfItems
        {
            get => _takenCountOfItems;
            set
            {
                _takenCountOfItems = value;
                _text.text = _takenCountOfItems.ToString();
            }
        }

        private Image _image;
        private TextMeshProUGUI _text;
        private ItemSO _item;
        private int _takenCountOfItems;

        private void Awake()
        {
            _image = GetComponentInChildren<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Init(ItemSO item, int count, bool createPartial = false)
        {
            TakenCountOfItems = createPartial ? count / 2 : count;
            Item = item;

            _image.raycastTarget = false;
            (_image.sprite, _text.text) = (Item.Sprite, TakenCountOfItems.ToString());
        }

        void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}