using System;
using Inventory.Models;
using Mirror;
using UnityEngine;

namespace Inventory.Models
{
    public class ItemInWorld : NetworkBehaviour
    {
        public ItemSO Item
        {
            get => _item;
            set
            {
                _item = value;
                _spriteRenderer.sprite = _item.Sprite;
            }
        }

        public int Count { get; set; }

        private SpriteRenderer _spriteRenderer;
        private ItemSO _item;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void GetPickedUp()
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }
}