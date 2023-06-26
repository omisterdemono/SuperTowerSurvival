using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EdibleItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();

        public string ActionName => "Build";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            foreach (ModifierData data in modifiersData)
            {
                Debug.Log("red");
                Debug.Log(data);
                //data.statModifier.AffectCharacter(character, data.value);
            }
            return true;
        }
    }

    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]
    public class ModifierData
    {
        //ublic CharacterStatModifierSO statModifier;
        public float value;
    }
}