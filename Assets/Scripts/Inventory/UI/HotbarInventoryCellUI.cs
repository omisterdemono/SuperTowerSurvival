using TMPro;
using UnityEngine;

namespace Inventory.UI
{
    public class HotbarInventoryCellUI : InventoryCellUI
    {
        [SerializeField] private TextMeshProUGUI _activateButtonHintText;

        public TextMeshProUGUI ActivateButtonHintText => _activateButtonHintText;
    }
}