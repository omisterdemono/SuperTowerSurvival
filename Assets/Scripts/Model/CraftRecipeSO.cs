using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Model
{
    [CreateAssetMenu]
    public class CraftRecipeSO : ScriptableObject
    {
        [SerializeField]
        private List<CraftItemSO> items = new List<CraftItemSO>();
        [SerializeField]
        private ItemSO item;
        [SerializeField]
        public InventorySO inventory;

        public void Craft()
        {
            foreach(var i in items)
            {
                inventory.RemoveItem(i.item.item, i.needQuantity);
            }
            inventory.AddItem(item, 1);
        }

    }


    
}