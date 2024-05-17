using System;
using System.Linq;
using Inventory.Models;
using Inventory.UI;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory.Tests
{
    public class InventoryTester : MonoBehaviour
    {
        [SerializeField] private PlayerInventory _playerInventory;

        private PlayerInventory PlayerInventory
        {
            get
            {
                if (_playerInventory == null)
                {
                    _playerInventory = FindObjectsOfType<PlayerInventory>().First(x => x.isOwned);
                }

                return _playerInventory;
            }
            set => _playerInventory = value;
        }

        [Header("Testing")] [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private int _itemCount;
        [SerializeField] private TMP_InputField _tmpInput;


        private void Start()
        {
        }

        public void AddItem()
        {
            if (_tmpInput.text == "instruments")
            {
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "shovel"), 1);
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "iron_axe"), 1);
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "iron_pickaxe"), 1);
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "build_hammer"), 1);
                return;
            }

            if (_tmpInput.text == "")
            {
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "wood_wall"), 8);
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "defence_gun"), 4);
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "heal_potion"), 4);
                PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == "damage_potion"), 4);
                return;
            }

            PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == _tmpInput.text), _itemCount);
        }

        public void RemoveItem()
        {
            PlayerInventory.Inventory.TryRemoveItem(_itemDatabase.Items.First(i => i.Id == _tmpInput.text), _itemCount);
        }
    }
}