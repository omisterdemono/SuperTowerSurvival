using System;
using Inventory.Models.ItemActions;
using UnityEngine;

namespace Inventory.Models
{
    [CreateAssetMenu(menuName = "Inventory/UsableItem")]
    public class UsableItemSO : ItemSO
    {
        public ItemAction ItemAction;
        public AudioClip ActionSFX;

        public virtual void PerformAction(Character character, Action afterPerform)
        {
            ItemAction.PerformAction(character, this, afterPerform);
        }
    }
}