using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item")]
    public class Item : ScriptableObject
    {
        public Image ItemImage;
        public int MaxStackSize;
        public GameObject ItemPrefab;
    }
}
