using System.Collections.Generic;
using Inventory.UI;
using UnityEngine;

public class HotBar : MonoBehaviour
{
    public List<InventoryCellUI> HotbarCells { get; set; } = new();
    public int SelectedCell { get; private set; } = 0;

    public void SelectCell(int cellId)
    {
        HotbarCells[SelectedCell].SetSelect(false);

        if (cellId != -1)
        {
            HotbarCells[cellId].SetSelect(true);
        }

        SelectedCell = cellId;
    }

    public bool ActivateCell(int id, Character character)
    {
        var instrumentItem = HotbarCells[id].InventoryCell.Item as InstrumentItemSO;
        if (instrumentItem != null)
        {
            instrumentItem.PerformAction(character, () => { });
        }

        SelectCell(id);
        return instrumentItem != null;
    }
}