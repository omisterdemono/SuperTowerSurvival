using Assets.Scripts.Weapons;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolderScript : NetworkBehaviour
{
    [SerializeField] private List<GameObject> _toolSlots = new List<GameObject> ();
    private int _equipedSlot = 0;

    public void SetIcons(List<GameObject> tools)
    {
        for (int i = 0; i < _toolSlots.Count; i++)
        {
            _toolSlots[i].GetComponent<Image>().sprite = tools[i].GetComponentInChildren<SpriteRenderer>().sprite;
        }
    }
    public void ChangeSlot(int slot)
    {
        _toolSlots[_equipedSlot].transform.GetChild(0).GetComponent<Image>().enabled = false;
        _toolSlots[slot].transform.GetChild(0).GetComponent<Image>().enabled = true;
        _equipedSlot = slot;
    }
}