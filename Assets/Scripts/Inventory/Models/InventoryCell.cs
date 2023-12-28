using System;
using Inventory.Model;

namespace Inventory.Models
{
    public class InventoryCell
    {
        public ItemSO Item { get; set; }
        public int Count { get; set; }
        public bool IsFull => Item != null && Item.MaxStackSize == Count;
        public int AvailableCount => Item.MaxStackSize - Count;

        public Action<InventoryCell> Modified;
    }
}