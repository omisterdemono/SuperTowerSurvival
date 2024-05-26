using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory.UI
{
    public class CraftingUI : MonoBehaviour
    {
        [Header("Infrastructure")] [SerializeField]
        private CraftingDatabaseSO _craftingDatabase;

        [SerializeField] private Transform _categoriesHolder;
        [SerializeField] private Transform _recipesHolder;

        [Header("Recipe properties setup")] [SerializeField]
        private RecipePropertiesUI _recipePropertiesUI;

        [SerializeField] private Color _requiredItemsAvailableColor;
        [SerializeField] private Color _lackRequiredItemsColor;

        [Header("Prefabs")] [SerializeField] private CraftCategoryUI _categorySlotPrefab;
        [SerializeField] private GameObject _contentPrefab;
        [SerializeField] private CraftRecipeUI _recipeSlotPrefab;

        private Inventory _inventory;
        private CraftingSystem _craftingSystem;

        private List<GameObject> _categoriesContent = new List<GameObject>();
        private List<CraftCategoryUI> _categoriesSlots = new List<CraftCategoryUI>();

        private void Awake()
        {
            InitCategories();
            InitRecipes();
        }

        private void InitCategories()
        {
            for (var index = 0; index < _craftingDatabase.CategoryIcons.Length; index++)
            {
                var categorySprite = _craftingDatabase.CategoryIcons[index];
                var categorySlot = Instantiate(_categorySlotPrefab, _categoriesHolder);
                categorySlot.Init(categorySprite, index, SwitchCategory);
                _categoriesSlots.Add(categorySlot);

                var content = Instantiate(_contentPrefab, _recipesHolder);
                _categoriesContent.Add(content);

                if (index != 0)
                {
                    content.SetActive(false);
                }
            }
        }

        private void SwitchCategory(int index)
        {
            _recipePropertiesUI.gameObject.SetActive(false);
            for (int i = 0; i < _categoriesSlots.Count; i++)
            {
                _categoriesContent[i].SetActive(i == index);
            }
        }

        private void InitRecipes()
        {
            for (var index = 0; index < _craftingDatabase.CategoriesNames.Length; index++)
            {
                var categoryRecipes = _craftingDatabase.CraftRecipes
                    .Where(c => c.Category == _craftingDatabase.CategoriesNames[index])
                    .ToList();

                foreach (var recipe in categoryRecipes)
                {
                    var recipeUI = Instantiate(_recipeSlotPrefab, _categoriesContent[index].transform);
                    recipeUI.Init(recipe, SetRecipeProperties);
                }
            }
            
            _recipePropertiesUI.gameObject.SetActive(false);
        }

        public void AttachInventory(PlayerInventory playerInventory)
        {
            _inventory = playerInventory.Inventory;
            _inventory.InventoryChanged += UpdateRecipeProperties;

            _craftingSystem = playerInventory.CraftingSystem;
            _recipePropertiesUI.ItemCrafted += _craftingSystem.CraftItem;
        }

        private void SetRecipeProperties(CraftRecipeSO recipe)
        {
            if (_recipePropertiesUI.gameObject.activeSelf == false)
            {
                _recipePropertiesUI.gameObject.SetActive(true);
            }

            var (requiredItemsCounts, requiredItemsColors) = GetRequiredItemsCountsAndTextColors(recipe);

            _recipePropertiesUI.Init(recipe, requiredItemsCounts.ToArray(), requiredItemsColors.ToArray(),
                _craftingSystem.ItemCanBeCrafted(recipe));
        }

        private (List<string>, List<Color>) GetRequiredItemsCountsAndTextColors(CraftRecipeSO recipe)
        {
            var requiredItemsCounts = new List<string>();
            var requiredItemsColors = new List<Color>();

            for (var i = 0; i < recipe.Items.Count; i++)
            {
                var item = recipe.Items[i];
                var itemCount = _inventory.ItemCount(item);

                var requiredItem = $@"{itemCount}/{recipe.ItemCounts[i]}";
                requiredItemsCounts.Add(requiredItem);
                requiredItemsColors.Add(itemCount < recipe.ItemCounts[i]
                    ? _lackRequiredItemsColor
                    : _requiredItemsAvailableColor);
            }

            return (requiredItemsCounts, requiredItemsColors);
        }

        private void UpdateRecipeProperties()
        {
            if (!_recipePropertiesUI.gameObject.activeSelf)
            {
                return;
            }

            var (requiredItemsCounts, requiredItemsColors) =
                GetRequiredItemsCountsAndTextColors(_recipePropertiesUI.RecipeSo);
            _recipePropertiesUI.UpdateRequiredItemsCounts(requiredItemsCounts.ToArray(), requiredItemsColors.ToArray(),
                _craftingSystem.ItemCanBeCrafted);
        }

        private void OnEnable()
        {
            UpdateRecipeProperties();
        }
    }
}