using Inventory.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        
        public bool IsGettingMoved { get; set; }

        public Image Image => _image;

        public TextMeshProUGUI Text => _text;

        public bool IsAssigned { get; set; } = false;

        private void Awake()
        {
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