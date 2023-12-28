using System;
using UnityEngine;

namespace Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int _count = 18;
        private Inventory _inventory;

        private void Awake()
        {
            _inventory = new Inventory(_count);
        }
        
        
    }
}
