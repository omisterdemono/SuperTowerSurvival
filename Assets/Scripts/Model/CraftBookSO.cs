using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Model
{
    [CreateAssetMenu]
    public class CraftBookSO : ScriptableObject
    {
        [SerializeField]
        public List<CraftRecipeSO> craftRecipes;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;


        public void UpdateCraft(InventorySO inventory)
        {
            foreach (var craft in craftRecipes)
                craft.UpdateCraftItems(inventory);
        }
    }


}