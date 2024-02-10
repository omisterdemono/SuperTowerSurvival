﻿using System;
using System.Linq;
using Inventory.Models;
using Inventory.UI;
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
                    _playerInventory = FindObjectOfType<PlayerInventory>();
                }
                return _playerInventory;
            }
            set => _playerInventory = value;
        }

        [Header("Testing")]
        [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private int _itemCount;
        [SerializeField] private TMP_InputField _tmpInput;

        public void AddItem()
        {
            PlayerInventory.LastMoveDirection = new Vector3(1.0f, 1.0f);
            
            PlayerInventory.Inventory.TryAddItem(_itemDatabase.Items.First(i => i.Id == _tmpInput.text), _itemCount);
        }
        
        public void RemoveItem()
        {
            PlayerInventory.Inventory.TryRemoveItem(_itemDatabase.Items.First(i => i.Id == _tmpInput.text), _itemCount);
        }
    }
}