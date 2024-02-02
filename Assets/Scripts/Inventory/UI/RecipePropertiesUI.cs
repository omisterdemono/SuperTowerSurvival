using System;
using System.Collections.Generic;
using CodeSmile.Extensions;
using Inventory.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class RecipePropertiesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _itemDescription;
        [SerializeField] private Transform _requiredItems;
        [SerializeField] private RequiredItemUI _requiredItemPrefab;
        [SerializeField] private Button _craftButton;

        private CraftRecipeSO _recipeSo;
        private List<RequiredItemUI> _requiredItemsUI = new List<RequiredItemUI>();

        public Action<CraftRecipeSO> ItemCrafted;

        public CraftRecipeSO RecipeSo => _recipeSo;

        public void Init(CraftRecipeSO recipe, string[] requiredItemsCounts, Color[] requiredItemsColors,
            bool canBeCrafted)
        {
            if (recipe == _recipeSo)
            {
                return;
            }
            
            _requiredItems.DestroyAllChildren();
            _requiredItemsUI.Clear();
            _craftButton.onClick.RemoveAllListeners();

            _itemName.text = recipe.ResultItem.Name;
            _itemDescription.text = recipe.ResultItem.Description;
            _recipeSo = recipe;
            
            _craftButton.interactable = canBeCrafted;

            for (var i = 0; i < _recipeSo.Items.Count; i++)
            {
                var requiredItem = Instantiate(_requiredItemPrefab, _requiredItems);
                requiredItem.ItemImage.sprite = _recipeSo.Items[i].Sprite;
                requiredItem.ItemCountText.text = requiredItemsCounts[i];
                requiredItem.ItemCountText.color = requiredItemsColors[i];

                _requiredItemsUI.Add(requiredItem);
            }

            _craftButton.onClick.AddListener(() =>
            {
                ItemCrafted.Invoke(_recipeSo);
            });
        }

        public void UpdateRequiredItemsCounts(string[] requiredItemsCounts, Color[] requiredItemsColors,
            Func<CraftRecipeSO, bool> defineCanBeCrafted)
        {
            _craftButton.interactable = defineCanBeCrafted.Invoke(_recipeSo);

            for (var i = 0; i < _requiredItemsUI.Count; i++)
            {
                _requiredItemsUI[i].ItemCountText.text = requiredItemsCounts[i];
                _requiredItemsUI[i].ItemCountText.color = requiredItemsColors[i];
            }
        }
    }
}