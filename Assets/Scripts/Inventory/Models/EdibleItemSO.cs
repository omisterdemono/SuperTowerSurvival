using System;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

namespace Inventory.Models
{
    [CreateAssetMenu]
    public class UsableItemSO : ItemSO, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();

        public string ActionName => "Build";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        public void PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {

        }
    }

    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        void PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]
    public class ModifierData
    {
        //ublic CharacterStatModifierSO statModifier;
        public float value;
    }
}