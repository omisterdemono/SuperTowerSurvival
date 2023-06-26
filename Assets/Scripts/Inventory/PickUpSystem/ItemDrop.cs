using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Globalization;

namespace Assets.Scripts.PickUpSystem
{
    public class ItemDrop : NetworkBehaviour
    {

        public ItemSO inventoryItem;
        [SerializeField]
        private ItemsSO AllItems;
        [SyncVar]
        public int idOfItem;
        [SyncVar]
        public int Quantity  = 1;


        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private float duration = 0.3f;

        private void Start()
        {
            inventoryItem= AllItems.GetItem(idOfItem);
            GetComponent<SpriteRenderer>().sprite = inventoryItem.ItemImage;
        }
        [Command(requiresAuthority =false)]
        public void DestroyItem()
        {
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(AnimateItemPickup());
        }
        public void SetItem(int id, int quantity)
        {
            inventoryItem = AllItems.GetItem(id);
            idOfItem = id;
            Quantity = quantity;
        }



        private IEnumerator AnimateItemPickup()
        {
            
            audioSource.Play();
            Vector3 startScale = transform.localScale;
            Vector3 endScale = Vector3.zero;
            float currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                transform.localScale =
                    Vector3.Lerp(startScale, endScale, currentTime / duration);
                yield return null;
            }
            NetworkServer.Destroy(gameObject);
            
        }
    }
}