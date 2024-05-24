using System.Collections.Generic;
using Inventory;
using Inventory.Models;
using Inventory.UI;
using UnityEngine;

public class HotBar : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    public List<InventoryCellUI> HotbarCells { get; set; } = new();
    public int SelectedCell { get; private set; } = 0;

    public PlayerInventory PlayerInventory
    {
        get => _playerInventory;
        set => _playerInventory = value;
    }

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
        var usableItem = HotbarCells[id].InventoryCell.Item as UsableItemSO;
        if (usableItem != null)
        {
            usableItem.PerformAction(PlayerInventory, () => { });
        }

        SelectCell(id);
        return usableItem != null;
    }
}