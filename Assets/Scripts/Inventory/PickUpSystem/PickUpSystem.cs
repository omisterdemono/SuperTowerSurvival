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
        private InventoryController controller;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isOwned)
            {
                ItemDrop item = collision.GetComponent<ItemDrop>();
                if (item != null)
                {
                    int reminder = controller.inventoryData.AddItem(item.inventoryItem, item.Quantity);
                    if (reminder == 0)
                        item.DestroyItem();
                    else
                        item.Quantity = reminder;
                }
            }

        }
    }
}