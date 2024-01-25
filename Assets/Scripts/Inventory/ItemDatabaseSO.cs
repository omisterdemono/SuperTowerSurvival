using System.Collections.Generic;
using Inventory.Models;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Database")]
    public class ItemDatabaseSO : ScriptableObject
    {
        public List<ItemSO> Items;
    }
}