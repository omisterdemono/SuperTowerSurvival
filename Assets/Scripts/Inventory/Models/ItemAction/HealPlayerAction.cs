using System;
using UnityEngine;

namespace Inventory.Models.ItemAction
{
    [CreateAssetMenu(menuName = "Actions/Heal player")]
    public class HealPlayerAction : ItemAction
    {
        public float Amount;

        public override void PerformAction(Character character, ItemSO holderItem, Action afterPerform = null)
        {
            character.Health.Heal(Amount);
        }
    }
}