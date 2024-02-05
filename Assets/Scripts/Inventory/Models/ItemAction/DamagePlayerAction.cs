using UnityEngine;

namespace Inventory.Models.ItemActions
{
    [CreateAssetMenu(menuName = "Actions/Damage player")]
    public class DamagePlayerAction : ItemAction
    {
        
        public float Amount;
        public override void PerformAction(Character character, ItemSO holderItem)
        {
            character.Health.Damage(Amount);
        }
    }
}