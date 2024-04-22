using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class RequiredItemUI : MonoBehaviour
    {
        public Image ItemImage => _itemImage;
        public TMP_Text ItemCountText => _itemCountText;
        
        private Image _itemImage;
        private TMP_Text _itemCountText;
        
        private void Awake()
        {
            _itemImage = GetComponentInChildren<Image>();
            _itemCountText = GetComponentInChildren<TMP_Text>();
        }
    }
}