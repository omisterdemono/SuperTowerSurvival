﻿using System;
using UnityEngine;

namespace Inventory.Models.ItemAction
{
    [CreateAssetMenu(menuName = "Actions/Select instrument")]
    public class SelectInstrumentAction : ItemAction
    {
        public override void PerformAction(Character character, ItemSO holderItem, Action afterPerform = null)
        {
            character.SelectInstrumentById(holderItem.Id);
        }
    }
}