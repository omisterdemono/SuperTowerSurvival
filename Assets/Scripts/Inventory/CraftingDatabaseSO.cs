using System.Collections.Generic;
using Inventory.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Crafting/Database")]
    public class CraftingDatabaseSO : ScriptableObject
    {
        public Sprite[] CategoryIcons;
        [FormerlySerializedAs("Categories")] [FormerlySerializedAs("CategoryNames")] public string[] CategoriesNames;
        
        public List<CraftRecipeSO> CraftRecipes = new();
    }
}