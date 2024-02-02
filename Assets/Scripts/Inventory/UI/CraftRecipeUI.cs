using System;
using Inventory.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class CraftRecipeUI : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private Button _setPropertiesButton;

        private CraftRecipeSO _craftRecipeSo;

        public void Init(CraftRecipeSO recipe, Action<CraftRecipeSO> setRecipeProperties)
        {
            _craftRecipeSo = recipe;
            _itemImage.sprite = recipe.ResultItem.Sprite;
            _setPropertiesButton.onClick.AddListener(() => setRecipeProperties.Invoke(_craftRecipeSo));
        }
    }
}