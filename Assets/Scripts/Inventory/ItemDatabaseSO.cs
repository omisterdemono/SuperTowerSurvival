using System.Collections.Generic;
using System.Linq;
using Inventory.Models;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Database")]
    public class ItemDatabaseSO : ScriptableObject
    {
        public List<ItemSO> Items;
        
        public ItemSO GetItemSOById(string itemId)
        {
            return Items.First(i => i.Id.Equals(itemId));
        }
    }
}