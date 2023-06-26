using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ItemsSO : ScriptableObject
    {
        [SerializeField]
        private List<ItemSO> items;


        public ItemSO GetItem(int id)
        {
            return items.FirstOrDefault(x=>x.IdOfItem==id);
        }


    }
}