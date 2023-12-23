using UnityEngine;


namespace Inventory.Model
{
    [CreateAssetMenu]
    public class CraftItemSO : ScriptableObject
    {
        [field: SerializeField]
        public ItemSO item { get; set; }

        public int ID => GetInstanceID();

        [field: SerializeField]
        public int needQuantity { get; set; } = 2;
    }
}