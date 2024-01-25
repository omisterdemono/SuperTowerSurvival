using UnityEngine;

namespace Inventory.Models
{
    [CreateAssetMenu(menuName = "Inventory/Structure")]
    public class StructureItemSO : UsableItemSO
    {
        public int Index;
        public GameObject StructurePrefab;
    }
}