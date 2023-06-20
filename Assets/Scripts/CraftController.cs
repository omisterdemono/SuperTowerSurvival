using Inventory.Model;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftController : NetworkBehaviour
{

    [SerializeField]
    private CraftBookUI craftUI;

    [SerializeField]
    private InventorySO inventoryData;


    [SerializeField]
    private AudioClip dropClip;




    private void Start()
    {
        if (isOwned)
        {
            PrepareUI();
        }
    }


    private void PrepareUI()
    {

        craftUI = GameObject.FindAnyObjectByType<CraftBookUI>();
        craftUI.InitializeCraftUI(2);
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
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (craftUI.isActiveAndEnabled == false)
            {
                craftUI.Show();
            }
            else
            {
                craftUI.Hide();
            }
        }
    }
}
