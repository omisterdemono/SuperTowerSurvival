using Inventory.Model;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSlotUI : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject resultPrefab;

    [SerializeField]
    private GameObject contentPanel;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;


    List<GameObject> listOfUIItems = new List<GameObject>();


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
    }
     public void SetData(CraftRecipeSO recipeSO)
    {
        for(int i=0; i < recipeSO.items.Count; i++)
        {
            listOfUIItems[i].GetComponent<CraftItemUI>().SetData(recipeSO.items[i].ItemImage, recipeSO.items[i].quantity, recipeSO.quantityOfItems[i]);
        }
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
