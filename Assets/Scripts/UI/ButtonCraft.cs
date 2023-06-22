using Inventory.Model;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCraft : NetworkBehaviour
{
    [SerializeField]
    public Button yourButton;


    private CraftRecipeSO craftRecipe;
    private InventorySO inventorySO;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(R);
    }
    public void Initialize(CraftRecipeSO craftRecipe, InventorySO inventorySO)
    {
        this.craftRecipe = craftRecipe;
        this.inventorySO = inventorySO;
    }
    private void R()
    {
        craftRecipe.Craft(inventorySO);
    }

}
