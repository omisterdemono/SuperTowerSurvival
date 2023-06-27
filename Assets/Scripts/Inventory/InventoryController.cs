using Assets.Scripts.PickUpSystem;
using Inventory.Model;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class InventoryController : NetworkBehaviour
{
    [SerializeField]
    private GameObject inventoryUI;

    [SerializeField]
    private GameObject craftUI;

    [SerializeField]
    public InventorySO inventoryData;

    [SerializeField]
    private CraftBookSO book;

    public List<InventoryItem> initialItems = new List<InventoryItem>();


    [SerializeField]
    private GameObject ItemPrefab;

    [SerializeField]
    private AudioClip dropClip;

    private void Start()
    {
        craftUI = GameObject.FindGameObjectWithTag("CraftUI");
        if (isOwned)
        {
            PrepareUI();
            PrepareInventoryData();
            craftUI.SetActive(true);
            craftUI.GetComponent<CraftBookUI>().UpdateData(book.craftRecipes, inventoryData);
        }
    }


    private void PrepareInventoryData()
    {
        inventoryData.Initialize();
        inventoryData.OnInventoryUpdated += UpdateInventoryUI;
        foreach (var item in initialItems)
        {
            if (item.IsEmpty)
                continue;
            inventoryData.AddItem(item);
        }
        book.UpdateCraft(inventoryData);
        craftUI.GetComponent<CraftBookUI>().UpdateData(book.craftRecipes, inventoryData);

    }

    private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
    {
        inventoryUI.GetComponent<UIInventoryPage>().ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryUI.GetComponent<UIInventoryPage>().UpdateData(item.Key, item.Value.item.ItemImage,
                item.Value.quantity);
        }
        book.UpdateCraft(inventoryData);
        craftUI.GetComponent<CraftBookUI>().UpdateData(book.craftRecipes, inventoryData);
    }

    private void PrepareUI()
    {
        inventoryUI = GameObject.FindGameObjectWithTag("Inventory");
        inventoryUI.GetComponent<UIInventoryPage>().InitializeInventoryUI(inventoryData.Size);

        inventoryUI.GetComponent<UIInventoryPage>().OnSwapItems += HandleSwapItems;
        inventoryUI.GetComponent<UIInventoryPage>().OnStartDragging += HandleDragging;
        inventoryUI.GetComponent<UIInventoryPage>().OnItemActionRequested += HandleItemActionRequest;

        craftUI.GetComponent<CraftBookUI>().InitializeCraftUI(book);
    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        if (inventoryData.Stack(itemIndex_1, itemIndex_2))
            return;
        inventoryData.SwapItems(itemIndex_1, itemIndex_2);
    }

    private void HandleDragging(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        inventoryUI.GetComponent<UIInventoryPage>().CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty|| !isOwned)
            return;

        
        inventoryUI.GetComponent<UIInventoryPage>().ShowItemAction(itemIndex);
        inventoryUI.GetComponent<UIInventoryPage>().AddAction("Build", () => PerformAction(itemIndex));
        

        IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
        var vector = new Vector3(transform.position.x, transform.position.y-2f);
        
        inventoryUI.GetComponent<UIInventoryPage>().AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity, vector));
        

    }
    public void PerformAction(int itemIndex)
    {
        
    }
    private void DropItem(int itemIndex, int quantity, Vector3 vector)
    {
        int idOfItem = inventoryData.GetItemAt(itemIndex).item.IdOfItem;
        inventoryData.RemoveItem(itemIndex, quantity);
        inventoryUI.GetComponent<UIInventoryPage>().ResetSelection();
        DropItemOnServer(idOfItem, quantity, vector);

    }

    [Command(requiresAuthority = false)]
    public void DropItemOnServer(int itemIndex, int quantity, Vector3 vector)
    {
        GameObject uiItem =
            Instantiate(ItemPrefab, vector, Quaternion.identity);
        //var i= inventoryData.GetItemAt(itemIndex);
        uiItem.GetComponent<ItemDrop>().SetItem(itemIndex, quantity);

        //SpawnOnClient(uiItem);
        NetworkServer.Spawn(uiItem);
    }



    [ClientRpc]
    private void SpawnOnClient(GameObject gameObject)
    {
        Instantiate(gameObject);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.GetComponent<UIInventoryPage>().isActiveAndEnabled == false)
            {
                inventoryUI.GetComponent<UIInventoryPage>().Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.GetComponent<UIInventoryPage>().UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else
            {
                inventoryUI.GetComponent<UIInventoryPage>().Hide();
            }

        }
        else if (Input.GetKeyDown(KeyCode.Tab) && isOwned)
        {
            if (craftUI.GetComponent<CraftBookUI>().isActiveAndEnabled == false)
            {
                craftUI.GetComponent<CraftBookUI>().Show();
            }
            else
            {
                craftUI.GetComponent<CraftBookUI>().Hide();
            }
        }
    }
}
