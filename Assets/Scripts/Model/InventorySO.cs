using Inventory.Model;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> inventoryItems;

        [field: SerializeField]
        public int Size { get; private set; } = 12;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public void RemoveItem(ItemSO item, int amount, List<ItemParameter> itemState = null)
        {
            var items = inventoryItems.FindAll(x =>
            {
                if (x.IsEmpty)
                    return false;
                return x.item.name == item.name;
            }).ToList();
            int quantity = 0;
            foreach (var i in items)
            {
                quantity += i.quantity;
            }
            if (quantity < amount)
                return;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].quantity - amount >= 0)
                {
                    RemoveItem(inventoryItems.FindLastIndex(x =>
                    {
                        if (x.IsEmpty)
                            return false;
                        return x.item.name == item.name;
                    }), amount);
                    break;
                }
                else
                {
                    int q = amount - items[i].quantity;
                    RemoveItem(inventoryItems.FindLastIndex(x =>
                    {
                        if (x.IsEmpty)
                            return false;
                        return x.item.name == item.name;
                    }), amount);
                    amount = q;
                }
            }
            InformAboutChange();
        }

        public int GetQuantityOfItem(ItemSO item)
        {
            var res = inventoryItems.FindAll(x =>
            {
                if (x.IsEmpty)
                    return false;
                return x.item.name == item.name;
            });

            int q = 0;
            foreach (var i in res)
            {
                q += i.quantity;
            }
            return q;
        }

        public bool CheckRecipe(CraftRecipeSO recipeSO)
        {
            bool check = true;
            for (int i = 0; i < recipeSO.items.Count; i++)
            {
                if (recipeSO.items[i].quantity < recipeSO.quantityOfItems[i])
                {
                    check = false;
                    break;
                }
            }

            return check;
        }

        public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            if (item.IsStackable == false)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    while (quantity > 0 && IsInventoryFull() == false)
                    {
                        quantity -= AddItemToFirstFreeSlot(item, 1, itemState);
                    }
                    InformAboutChange();
                    return quantity;
                }
            }
            quantity = AddStackableItem(item, quantity);
            InformAboutChange();
            return quantity;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int quantity
            , List<ItemParameter> itemState = null)
        {
            InventoryItem newItem = new InventoryItem
            {
                item = item,
                quantity = quantity,
                itemState =
                new List<ItemParameter>(itemState == null ? item.DefaultParametersList : itemState)
            };

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        private bool IsInventoryFull()
            => inventoryItems.Where(item => item.IsEmpty).Any() == false;

        private int AddStackableItem(ItemSO item, int quantity)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;
                if (inventoryItems[i].item.ID == item.ID)
                {
                    int amountPossibleToTake =
                        inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;

                    if (quantity > amountPossibleToTake)
                    {
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                    }
                    else
                    {
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].quantity + quantity);
                        InformAboutChange();
                        return 0;
                    }
                }
            }
            while (quantity > 0 && IsInventoryFull() == false)
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
            }
            return quantity;
        }

        public void RemoveItem(int itemIndex, int amount)
        {
            if (inventoryItems.Count > itemIndex)
            {
                if (inventoryItems[itemIndex].IsEmpty)
                    return;
                int reminder = inventoryItems[itemIndex].quantity - amount;
                if (reminder <= 0)
                    inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
                else
                    inventoryItems[itemIndex] = inventoryItems[itemIndex]
                        .ChangeQuantity(reminder);

                InformAboutChange();
            }
        }

        public void AddItem(InventoryItem item)
        {
            AddItem(item.item, item.quantity);
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue =
                new Dictionary<int, InventoryItem>();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;
                returnValue[i] = inventoryItems[i];
            }
            return returnValue;
        }


        public InventoryItem GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }

        public bool Stack(int itemIndex_1, int itemIndex_2)
        {
            if (inventoryItems[itemIndex_1].item.IsStackable && inventoryItems[itemIndex_2].item != null)
            {
                if (inventoryItems[itemIndex_1].item.Name == inventoryItems[itemIndex_2].item.Name)
                {
                    int maxQuantity = inventoryItems[itemIndex_1].item.MaxStackSize;
                    int sumOfQuantities = inventoryItems[itemIndex_1].quantity + inventoryItems[itemIndex_2].quantity;
                    if (sumOfQuantities <= maxQuantity)
                    {
                        inventoryItems[itemIndex_2] = inventoryItems[itemIndex_2].ChangeQuantity(sumOfQuantities);
                        inventoryItems[itemIndex_1] = InventoryItem.GetEmptyItem();
                    }
                    else
                    {
                        inventoryItems[itemIndex_2] = inventoryItems[itemIndex_2].ChangeQuantity(maxQuantity);
                        inventoryItems[itemIndex_1] = inventoryItems[itemIndex_1].ChangeQuantity(sumOfQuantities - maxQuantity);
                    }
                    InformAboutChange();
                    return true;
                }
            }
            return false;

        }

        public void SwapItems(int itemIndex_1, int itemIndex_2)
        {

            InventoryItem item1 = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = item1;
            InformAboutChange();
        }

        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }

    [Serializable]
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;
        public List<ItemParameter> itemState;
        public bool IsEmpty => item == null;

        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
                itemState = new List<ItemParameter>(this.itemState)
            };
        }

        public static InventoryItem GetEmptyItem()
            => new InventoryItem
            {
                item = null,
                quantity = 0,
                itemState = new List<ItemParameter>()
            };
    }
}