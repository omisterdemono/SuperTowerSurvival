using UnityEngine;

namespace Inventory.Models
{
    [CreateAssetMenu(menuName = "Inventory/Structure")]
    public class StructureItemSO : UsableItemSO
    {
        public GameObject StructurePrefab;
    }
}