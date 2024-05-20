using System;
using UnityEngine;

namespace Inventory.Models
{
    [CreateAssetMenu(menuName = "Inventory/UsableItem")]
    public class UsableItemSO : ItemSO
    {
        public ItemAction.ItemAction ItemAction;
        public AudioClip ActionSFX;
        public bool RemoveAfterUsing;

        public void PerformAction(PlayerInventory playerInventory, Action afterPerform)
        {
            ItemAction.PerformAction(playerInventory.Character, this, afterPerform);

            if (RemoveAfterUsing)
            {
                playerInventory.Inventory.TryRemoveItem(this, 1);
            }
        }
    }
}