using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftItemUI : NetworkBehaviour
{
    [SerializeField]
    public Image itemImage;
    [SerializeField]
    private TMP_Text quantityTxt;

    [SerializeField]
    private Image borderImage;

    
    private bool empty = true;

    public void Awake()
    {
        ResetData();
        Deselect();
    }
    public void ResetData()
    {
        itemImage.gameObject.SetActive(false);
        empty = true;
    }
    public void Deselect()
    {
        borderImage.enabled = false;
    }
    public void SetData(Sprite sprite, int? quantity, int? quantityNeed)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        if (quantityNeed == null ||quantity==null)
            quantityTxt.text = "";
        else
            quantityTxt.text = quantity + "/"+quantityNeed;
        empty = false;
    }

    public void Select()
    {
        borderImage.enabled = true;
    }
    


}
