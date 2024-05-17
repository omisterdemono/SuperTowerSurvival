using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class SlotScript : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon; 
        [SerializeField] private Text _stackSize;
        private Item _item;

        public void AddItem(Item item, int count)
        {
            _itemIcon.sprite = item.ItemImage.sprite;
            _stackSize.text = count.ToString();
        }

        public void RemoveItem()
        {
            _itemIcon.sprite = null;
            _stackSize.text = "";
        }
    }
}
