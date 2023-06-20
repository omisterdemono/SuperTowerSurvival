using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSlotUI : MonoBehaviour
{
    [SerializeField]
    private CraftItemUI itemPrefab;
    [SerializeField]
    private UIInventoryItem resultPrefab;




    List<CraftItemUI> listOfUIItems = new List<CraftItemUI>();
    UIInventoryItem result;

    private int currentlyDraggedItemIndex = -1;

    
    public void Show()
    {
        gameObject.SetActive(true);
    }





    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
