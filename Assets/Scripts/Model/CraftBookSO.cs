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


        [field: SerializeField]
        public int Size { get; private set; } = 2;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;


        public void UpdateCraft(InventorySO inventory)
        {
            foreach (var craft in craftRecipes)
                craft.UpdateCraftItems(inventory);
        }
    }


}