using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Models
{
    [CreateAssetMenu]
    public class CraftRecipeSO : ScriptableObject
    {
        [SerializeField]
        public List<ItemSO> items = new List<ItemSO>();
        [SerializeField]
        public List<int> quantityOfItems = new List<int>();
        [SerializeField]
        public ItemSO itemRes;
    }
}