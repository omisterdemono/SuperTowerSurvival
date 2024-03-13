using System.Collections.Generic;
using Inventory.UI;
using UnityEngine;

public class HotBar : MonoBehaviour
{
    public List<InventoryCellUI> HotbarCells { get; set; } = new();
    private int _selectedCell = 0;

    public void SelectCell(int cellId)
    {
        HotbarCells[_selectedCell].SetSelect(false);

        if (cellId != -1)
        {
            HotbarCells[cellId].SetSelect(true);
        }

        _selectedCell = cellId;
    }
}