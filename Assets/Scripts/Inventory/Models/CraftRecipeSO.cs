using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Models
{
    [CreateAssetMenu(menuName = "Inventory/Crafting/CraftRecipe")]
    public class CraftRecipeSO : ScriptableObject
    {
        public List<ItemSO> Items;
        public List<int> ItemCounts;
        public ItemSO ResultItem;
        public int ResultItemCount;
        public string Category;
    }
}