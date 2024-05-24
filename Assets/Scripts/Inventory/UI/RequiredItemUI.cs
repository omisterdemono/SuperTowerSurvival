using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class RequiredItemUI : MonoBehaviour
    {
        public Image ItemImage => _itemImage;
        public TMP_Text ItemCountText => _itemCountText;
        
        [SerializeField] private Image _itemImage;
        [SerializeField] private TMP_Text _itemCountText;
    }
}