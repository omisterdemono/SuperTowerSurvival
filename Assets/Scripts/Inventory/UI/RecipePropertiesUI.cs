using System;
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

        public Action ItemCrafted;

        void Init(CraftRecipeSO recipe, string[] requiredItemsCounts)
        {
            _itemName.text = recipe.ResultItem.Name;
            _itemDescription.text = recipe.ResultItem.Description;

            for (int i = 0; i < recipe.Items.Count; i++)
            {
                var requiredItem = Instantiate(_requiredItemPrefab, _requiredItems);
                requiredItem.ItemImage.sprite = recipe.Items[i].Sprite;
                requiredItem.ItemCountText.text = requiredItemsCounts[i];
            }

            _craftButton.onClick.AddListener(() => ItemCrafted.Invoke());
        }
    }
}