using Inventory.Model;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CraftBookUI : NetworkBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;

    private GameObject contentPanel;



    List<GameObject> listOfUIItems = new List<GameObject>();

    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging, OnStackRequest;

    public event Action<int, int> OnSwapItems;


    private void Start()
    {
        //Hide();
        contentPanel = GameObject.FindGameObjectWithTag("CraftHolder");
    }

    public void InitializeCraftUI(CraftBookSO bookSO)
    {
        //contentPanel = GameObject.FindGameObjectWithTag("CraftHolder");
        for (int i = 0; i < bookSO.craftRecipes.Count; i++)
        {
            var uiItem =
                Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.gameObject.transform.SetParent(contentPanel.transform);
            uiItem.GetComponent<CraftSlotUI>().InitializeSlotUI(bookSO.craftRecipes[i].items.Count);
            listOfUIItems.Add(uiItem);
        }
    }

    public void UpdateData(int itemIndex,
       CraftRecipeSO recipeSO)
    {
        if (listOfUIItems.Count > itemIndex)
        {
            listOfUIItems[itemIndex].GetComponent<CraftSlotUI>().SetData(recipeSO);
        }
    }

    public void UpdateData(List<CraftRecipeSO> recipeSO)
    {
        for(int i = 0;i<recipeSO.Count; i++)
        {
            UpdateData(i, recipeSO[i]);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}