using UnityEngine;

namespace Inventory.Models.ItemActions
{
    [CreateAssetMenu(menuName = "Actions/Heal player")]
    public class HealPlayerAction : ItemAction
    {
        public float Amount;
        public override void PerformAction(Character character, ItemSO holderItem)
        {
            character.Health.Heal(Amount);
        }
    }
}