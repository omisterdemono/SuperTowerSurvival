using Inventory.UI;
using UnityEngine;

namespace Infrastructure
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private InventoryUI _inventoryUIPrefab;

        public InventoryUI InitializeInventoryUI()
        {
            var canvas = FindObjectOfType<Canvas>();
            return Instantiate(_inventoryUIPrefab, canvas.transform);
        } 
    }
}