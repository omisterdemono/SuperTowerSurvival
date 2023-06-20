using Inventory.Model;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button1 : NetworkBehaviour
{
    [SerializeField]
    public Button yourButton;


    [SerializeField]
    public CraftRecipeSO craftRecipe;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(R);
    }
    private void R()
    {
        craftRecipe.Craft();
    }

}
