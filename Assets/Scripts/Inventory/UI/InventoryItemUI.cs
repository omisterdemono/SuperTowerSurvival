using Inventory.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class InventoryItemUI : MonoBehaviour
    {
        public bool IsGettingMoved { get; set; }
        private Image _image;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _image = GetComponentInChildren<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetItem(ItemSO item, int count)
        {
            _image.sprite = item.ItemImage;
            _text.text = count.ToString();
        }

        void Update()
        {
            if (IsGettingMoved)
            {
                transform.position = Input.mousePosition;
            }
        }

        public void SetIsGettingMoved(Transform newParent)
        {
            IsGettingMoved = !IsGettingMoved;
            _image.raycastTarget = !_image.raycastTarget;
            transform.SetParent(newParent, false);
        }
    }
}