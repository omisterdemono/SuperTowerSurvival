using System;
using UnityEngine;

namespace Inventory.Models.ItemActions
{
    [CreateAssetMenu(menuName = "Actions/Select structure")]
    public class SelectStructureAction : ItemAction
    {
        public override void PerformAction(Character character, ItemSO holderItem, Action afterPerform)
        {
            var structurePlacer = character.StructurePlacer;
            
            if(structurePlacer.TempItem == holderItem)
            {
                structurePlacer.CancelPlacement();
            }
            else if (structurePlacer.TempItem == null)
            {
                structurePlacer.SelectStructure(holderItem, afterPerform);
            }
        }
    }
}