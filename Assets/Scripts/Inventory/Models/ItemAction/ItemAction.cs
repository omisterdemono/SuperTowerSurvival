using System;
using UnityEngine;

namespace Inventory.Models.ItemAction
{
    public abstract class ItemAction : ScriptableObject
    {
        public string ActionName;
        public abstract void PerformAction(Character character, ItemSO holderItem, Action afterPerform = null);
    }
}