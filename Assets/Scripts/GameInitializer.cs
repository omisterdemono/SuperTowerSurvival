using Inventory.UI;
using UnityEngine;

namespace Infrastructure
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private InventoryUI _inventoryUIPrefab;
        [SerializeField] private CraftingUI _craftingUIPrefab;

        public InventoryUI InitializeInventoryUI()
        {
            var canvas = FindObjectOfType<Canvas>();
            return Instantiate(_inventoryUIPrefab, canvas.transform);
        } 
        
        public CraftingUI InitializeCraftingUI() 
        {
            var canvas = FindObjectOfType<Canvas>();
            return Instantiate(_craftingUIPrefab, canvas.transform);
        } 
    }
}