using System;
using UnityEngine;

namespace Inventory.Models.ItemActions
{
    public abstract class ItemAction : ScriptableObject
    {
        public string ActionName;
        public abstract void PerformAction(Character character, ItemSO holderItem, Action afterPerform = null);
    }
}