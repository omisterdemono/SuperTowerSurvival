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
    private UIInventoryPage inventoryUI;

    [SerializeField]
    private GameObject craftUI;

    [SerializeField]
    public InventorySO inventoryData;
    [SerializeField]
    private CraftBookSO book;

    public List<InventoryItem> initialItems = new List<InventoryItem>();

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
        inventoryUI.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                item.Value.quantity);
        }
        book.UpdateCraft(inventoryData);
        craftUI.GetComponent<CraftBookUI>().UpdateData(book.craftRecipes, inventoryData);
    }

    private void PrepareUI()
    {
        inventoryUI = GameObject.FindGameObjectWithTag("Inventory").GetComponent<UIInventoryPage>();
        inventoryUI.InitializeInventoryUI(inventoryData.Size);

        inventoryUI.OnSwapItems += HandleSwapItems;
        inventoryUI.OnStartDragging += HandleDragging;
        inventoryUI.OnItemActionRequested += HandleItemActionRequest;

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
        inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        //InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        //if (inventoryItem.IsEmpty)
        //    return;

        //IItemAction itemAction = inventoryItem.item as IItemAction;
        //if (itemAction != null)
        //{

        //    inventoryUI.ShowItemAction(itemIndex);
        //    inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
        //}

        //IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
        //if (destroyableItem != null)
        //{
        //    inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
        //}

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else
            {
                inventoryUI.Hide();
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
