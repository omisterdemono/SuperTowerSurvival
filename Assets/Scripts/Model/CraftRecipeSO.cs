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
        public List<ItemSO> items = new List<ItemSO>();
        [SerializeField]
        public List<int> quantityOfItems = new List<int>();
        [SerializeField]
        private ItemSO item;



        public void Craft(InventorySO inventory)
        {
            for(int i=0; i < items.Count; i++)
            {
                inventory.RemoveItem(items[i], quantityOfItems[i]);
            }
            inventory.AddItem(item, 1);
        }

        public void UpdateCraftItems(InventorySO inventory)
        {
            foreach(ItemSO item in items)
            {
                item.quantity= inventory.GetQuantityOfItem(item);
            }
        }
    }


    
}