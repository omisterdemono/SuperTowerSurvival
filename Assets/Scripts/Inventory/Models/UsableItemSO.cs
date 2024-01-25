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

        public void PerformAction(Character character)
        {
            ItemAction.PerformAction(character, this);
        }
    }
}