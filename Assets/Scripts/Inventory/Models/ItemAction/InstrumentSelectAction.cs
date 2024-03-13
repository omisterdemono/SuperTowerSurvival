using System;
using UnityEngine;

namespace Inventory.Models.ItemActions
{
    [CreateAssetMenu(menuName = "Actions/Damage player")]
    public class InstrumentSelectAction : ItemAction
    {
        public override void PerformAction(Character character, ItemSO holderItem, Action afterPerform)
        {
            throw new NotImplementedException();
        }
    }
}