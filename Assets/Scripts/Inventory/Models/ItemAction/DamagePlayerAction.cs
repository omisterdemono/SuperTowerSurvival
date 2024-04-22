using System;
using UnityEngine;

namespace Inventory.Models.ItemActions
{
    [CreateAssetMenu(menuName = "Actions/Damage player")]
    public class DamagePlayerAction : ItemAction
    {
        public float Amount;

        public override void PerformAction(Character character, ItemSO holderItem, Action afterPerform = null)
        {
            character.Health.Damage(Amount);
        }
    }
}