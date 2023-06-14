using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIInventoryPage : NetworkBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private string name;
    [SerializeField]
    private Sprite image;
    [SerializeField]
    private int count;

    [SerializeField]
    private MouseFollower mouseFollower;

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging;

    public event Action<int, int> OnSwapItems;


    public void InitializeInventoryUI(int inventorysize)
    {
        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItem uiItem =
                Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    public void Awake()
    {
        Hide();
        mouseFollower.Toggle(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        listOfUIItems[0].SetData(image, count);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }







    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {

    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        mouseFollower.Toggle(false);
    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {
        
    }
    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(image, count);

    }


    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        Debug.Log(inventoryItemUI.name);
    }
}