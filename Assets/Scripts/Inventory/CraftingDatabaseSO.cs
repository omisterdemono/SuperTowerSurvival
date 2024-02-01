using Inventory.Models;
using UnityEngine;

namespace Inventory
{
    public class CraftingDatabaseSO : ScriptableObject
    {
        public Sprite[] CategoryIcons;
        public string[] CategoryNames;
        
        public CraftRecipeSO[] CraftRecipes;
    }
}