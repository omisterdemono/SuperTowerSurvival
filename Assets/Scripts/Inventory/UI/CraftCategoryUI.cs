using System;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class CraftCategoryUI : MonoBehaviour
    {
        
        [SerializeField] private Image _itemImage;
        private Button _setPropertiesButton;
        
        private int _categoryIndex;

        public Button SetPropertiesButton
        {
            get => _setPropertiesButton;
            set => _setPropertiesButton = value;
        }

        public Image ItemImage
        {
            get => _itemImage;
            set => _itemImage = value;
        }

        private void Awake()
        {
            _setPropertiesButton = GetComponent<Button>();
        }

        public void Init(Sprite categorySprite, int index, Action<int> categorySwitch)
        {
            _itemImage.sprite = categorySprite;
            _categoryIndex = index;
            _setPropertiesButton.onClick.AddListener(() => categorySwitch.Invoke(_categoryIndex));
        }
    }
}