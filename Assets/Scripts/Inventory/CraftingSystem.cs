using Inventory.Models;

namespace Inventory
{
    public class CraftingSystem
    {
        private Inventory _inventory;

        public void CraftItem(CraftRecipeSO recipe)
        {
            for (int i = 0; i < recipe.Items.Count; i++)
            {
                _inventory.TryRemoveItem(recipe.Items[i], recipe.ItemCounts[i]);
            }

            _inventory.TryAddItem(recipe.ResultItem, recipe.ResultItemCount);
        }

        public (int, bool) CheckItemAvailability(ItemSO item, int countRequired)
        {
            var count = _inventory.ItemCount(item);
            return (count, count < countRequired);
        }
    }
}