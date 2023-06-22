using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Model
{
    [CreateAssetMenu]
    public class CraftItemSO : ScriptableObject
    {
        [field: SerializeField]
        public InventoryItem item { get; set; }

        public int ID => GetInstanceID();

        [field: SerializeField]
        public int needQuantity { get; set; } = 2;


    }

   
}