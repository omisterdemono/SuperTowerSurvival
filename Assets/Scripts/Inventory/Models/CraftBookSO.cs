using System;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Model
{
    [CreateAssetMenu]
    public class CraftBookSO : ScriptableObject
    {
        [SerializeField]
        public List<CraftRecipeSO> craftRecipes;

        public event Action<Dictionary<int, CraftItemSO>> OnInventoryUpdated;
    }
}