using UnityEngine;

namespace Inventory.Models
{
    [CreateAssetMenu(fileName = "Data", menuName = "Inventory/Instrument")]
    public class InstrumentItemSO : UsableItemSO
    {
        public float Strength;
        public float Durability;
        public InstrumentType InstrumentType;
    }
}
