using System;
using Inventory.Model;

namespace Inventory.Models
{
    public class InventoryCell
    {
        public ItemSO Item
        {
            get => _item;
            set
            {
                _item = value;
                Modified?.Invoke(this);
            }
        }

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                Modified?.Invoke(this);
            }
        }

        public bool IsFull => Item != null && Item.MaxStackSize == Count;
        public bool IsFree => Item == null && Count == 0;
        public int AvailableCount => Item.MaxStackSize - Count;

        public Action<InventoryCell> Modified;
        private ItemSO _item;
        private int _count;
    }
}