using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Database")]
    public class ItemDatabaseSO : ScriptableObject
    {
        public List<ItemSO> Items;
    }
}