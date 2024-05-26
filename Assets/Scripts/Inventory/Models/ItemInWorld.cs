using System;
using System.Linq;
using Inventory.Models;
using Mirror;
using UnityEngine;

namespace Inventory.Models
{
    public class ItemInWorld : NetworkBehaviour
    {
        public ItemSO Item { get; private set; }

        public int Count
        {
            get => _count;
            set => _count = value;
        }

        [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [SyncVar(hook = nameof(SetItem))] public string ItemId;
        [SyncVar] private int _count;

        [Command(requiresAuthority = false)]
        public void GetPickedUp()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void SetItem(string oldItemId, string newItemId)
        {
            var item = _itemDatabase.Items.FirstOrDefault(i => i.Id == newItemId) ?? throw new ArgumentNullException("itemId is incorrect");
            Item = item;
            _spriteRenderer.sprite = Item.Sprite;
        }
    }
}