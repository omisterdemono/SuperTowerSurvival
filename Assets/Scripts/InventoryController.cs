using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPage inventoryUI;

    public List<UIInventoryItem> initialItems = new List<UIInventoryItem>();

    [SerializeField]
    private AudioClip dropClip;

    public int size = 4;



    private void Start()
    {
        PrepareUI();
    }




    private void PrepareUI()
    {
        inventoryUI.InitializeInventoryUI(size);
    }



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();

            }
            else
            {
                inventoryUI.Hide();
            }

        }
    }
}
