using Inventory.Model;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PickUpSystem
{
    public class PickUpSystem : NetworkBehaviour
    {
        [SerializeField]
        private InventorySO inventoryData;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                int reminder = inventoryData.AddItem(item.InventoryItem, item.Quantity);
                if (reminder == 0)
                    item.DestroyItem();
                else
                    item.Quantity = reminder;
            }
        }
    }
}