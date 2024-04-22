using System;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class CraftCategoryUI : MonoBehaviour
    {
        
        [SerializeField] private Image _itemImage;
        private Button _switchCategoryButton;
        
        private int _categoryIndex;

        public Button SetPropertiesButton
        {
            get => _switchCategoryButton;
            set => _switchCategoryButton = value;
        }

        public Image ItemImage
        {
            get => _itemImage;
            set => _itemImage = value;
        }

        private void Awake()
        {
            _switchCategoryButton = GetComponent<Button>();
        }

        public void Init(Sprite categorySprite, int index, Action<int> categorySwitch)
        {
            _itemImage.sprite = categorySprite;
            _categoryIndex = index;
            _switchCategoryButton.onClick.AddListener(() => categorySwitch.Invoke(_categoryIndex));
        }
    }
}