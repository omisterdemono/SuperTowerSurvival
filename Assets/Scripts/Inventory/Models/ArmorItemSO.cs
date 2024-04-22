using UnityEngine;

namespace Inventory.Models
{
    public enum ArmorType
    {
        None, Head, Chest, Legs
    }
    
    [CreateAssetMenu(menuName = "Inventory/Armor")]
    public class ArmorItemSO : ItemSO
    {
        public ArmorType ArmorType;
        public int ProtectValue;
        public int Durability;
    }
}