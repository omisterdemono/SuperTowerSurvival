using System;
using UnityEngine;

namespace Inventory.Models.ItemActions
{
    [CreateAssetMenu(menuName = "Actions/Select instrument")]
    public class SelectInstrumentAction : ItemAction
    {
        public override void PerformAction(Character character, ItemSO holderItem, Action afterPerform)
        {
            character.SelectInstrumentById(holderItem.Id);
        }
    }
}