using UnityEngine;

namespace Inventory.Models.ItemActions
{
    [CreateAssetMenu(menuName = "Actions/Select structure")]
    public class SelectStructureAction : ItemAction
    {
        public override void PerformAction(Character character, ItemSO holderItem)
        {
            var structurePlacer = character.StructurePlacer;
            
            if(structurePlacer.CurrentItem == holderItem)
            {
                structurePlacer.CancelPlacement();
            }
            else if (structurePlacer.CurrentItem == null)
            {
                structurePlacer.SelectStructure(holderItem);
            }
        }
    }
}