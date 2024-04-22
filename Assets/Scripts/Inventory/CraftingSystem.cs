using Inventory.Models;
using Mirror.Examples.Basic;

namespace Inventory
{
    public class CraftingSystem
    {
        private Inventory _inventory;

        public CraftingSystem(Inventory inventory)
        {
            _inventory = inventory;
        }

        public void CraftItem(CraftRecipeSO recipe)
        {
            for (int i = 0; i < recipe.Items.Count; i++)
            {
                _inventory.TryRemoveItem(recipe.Items[i], recipe.ItemCounts[i]);
            }

            _inventory.TryAddItem(recipe.ResultItem, recipe.ResultItemCount);
        }

        public bool ItemCanBeCrafted(CraftRecipeSO recipeSo)
        {
            for (var i = 0; i < recipeSo.Items.Count; i++)
            {
                var (count, enough) = CheckItemAvailability(recipeSo.Items[i], recipeSo.ItemCounts[i]);

                if (!enough)
                {
                    return false;
                }
            }
            return true;
        }

        private (int, bool) CheckItemAvailability(ItemSO item, int countRequired)
        {
            var count = _inventory.ItemCount(item);
            return (count, count >= countRequired);
        }
    }
}