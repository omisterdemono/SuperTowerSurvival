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

        public Image Image => _image;

        public TextMeshProUGUI Text => _text;

        private TextMeshProUGUI _text;
        public bool IsAssigned { get; set; } = false;

        private void Awake()
        {
            _image = GetComponentInChildren<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();

            HandleItemShow();
        }

        private void HandleItemShow()
        {
            _image.gameObject.SetActive(IsAssigned);
            _text.gameObject.SetActive(IsAssigned);
        }

        public void SetItem(ItemSO item, int count)
        {
            IsAssigned = item != null;
            
            HandleItemShow();
            
            if (!IsAssigned)
            {
                return;
            }
            
            _image.sprite = item.Sprite;
            _text.text = count.ToString();
        }

        public (Sprite, string) CloneForMoving()
        {
            return (_image.sprite, _text.text);
        }
    }
}