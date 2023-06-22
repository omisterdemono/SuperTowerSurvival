using Inventory.Model;
using Mirror;
using Mirror.SimpleWeb;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSlotUI : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;

    [SerializeField]
    private GameObject contentPanel;

    [SerializeField]
    private GameObject resultPanel;

    [SerializeField]
    private GameObject button;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;


    List<GameObject> listOfUIItems = new List<GameObject>();
    private GameObject resultItem;


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void InitializeSlotUI(int craftsize)
    {
        //contentPanel = GameObject.FindGameObjectWithTag("CraftItems");
        for (int i = 0; i < craftsize; i++)
        {
            var uiItem =
                Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.gameObject.transform.SetParent(contentPanel.transform);

            listOfUIItems.Add(uiItem);
        }
        resultItem =
                Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        resultItem.gameObject.transform.SetParent(resultPanel.transform);

        
    }
     public void SetData(CraftRecipeSO recipeSO)
    {
        for(int i=0; i < recipeSO.items.Count; i++)
        {
            listOfUIItems[i].GetComponent<CraftItemUI>().SetData(recipeSO.items[i].ItemImage, recipeSO.items[i].quantity, recipeSO.quantityOfItems[i]);
        }
        resultItem.GetComponent<CraftItemUI>().SetData(recipeSO.itemRes.ItemImage, null, null);

    }
    public void SetButton(CraftRecipeSO recipeSO, InventorySO inventorySO)
    {
        button.GetComponent<ButtonCraft>().Initialize(recipeSO, inventorySO);
        if (inventorySO.CheckRecipe(recipeSO))
        {
            button.GetComponent<CanvasGroup>().alpha = 1.0f;
            button.GetComponent<CanvasGroup>().blocksRaycasts= true;
        }
        else
        {
            button.GetComponent<CanvasGroup>().alpha = 0.7f;
            button.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
