using System;
using Inventory.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class CraftRecipeUI : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        
        private Button _setPropertiesButton;
        private CraftRecipeSO _craftRecipeSo;

        public Button SetPropertiesButton => _setPropertiesButton;

        public Image ItemImage => _itemImage;

        private void Awake()
        {
            _setPropertiesButton = GetComponent<Button>();
            _itemImage = GetComponentInChildren<Image>();
        }

        public void Init(CraftRecipeSO recipe, Action<CraftRecipeSO> setRecipeProperties)
        {
            _itemImage.sprite = recipe.ResultItem.Sprite;
            _setPropertiesButton.onClick.AddListener(() => setRecipeProperties.Invoke(recipe));
        }
    }
}