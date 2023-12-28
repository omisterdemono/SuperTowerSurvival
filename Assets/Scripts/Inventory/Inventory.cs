using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Model;
using Inventory.Models;
using JetBrains.Annotations;

namespace Inventory
{
    public class Inventory
    {
        public InventoryCell[] Cells { get; private set; }

        public Action<int, InventoryCell> ItemAdded;
        public Action<int, InventoryCell> ItemRemoved;

        public bool IsEmpty => Cells.Any(c => c.Item == null);
        public bool IsFull => Cells.All(c => c.IsFull);

        public Inventory(int count)
        {
            Cells = new InventoryCell[count];

            for (int i = 0; i < count; i++)
            {
                Cells[i] = new InventoryCell()
                {
                    Item = null,
                    Count = 0
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="countToAdd"></param>
        /// <returns>Count of items that not fit into inventory</returns>
        /// <exception cref="ArgumentNullException">If item is null</exception>
        public int TryAddItem([NotNull] ItemSO item, int countToAdd)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var availableCellsWithSameItem = GetCellsWithSameItem(item);
            countToAdd = FillCells(item, countToAdd, availableCellsWithSameItem);

            if (countToAdd == 0)
            {
                return countToAdd;
            }

            var availableFreeCells = GetFreeCells();
            countToAdd = FillCells(item, countToAdd, availableFreeCells);

            return countToAdd;
        }

        public int TryAddToCell(ItemSO item, int index, int countToAdd)
        {
            countToAdd = FillCell(item, countToAdd, Cells[index]);

            return countToAdd;
        }

        private IEnumerable<InventoryCell> GetCellsWithSameItem(ItemSO item)
        {
            return Cells.Where(c => c.Item == item && !c.IsFull);
        }

        private int FillCells(ItemSO item, int countToAdd, IEnumerable<InventoryCell> availableCellsWithSameItem)
        {
            foreach (var cell in availableCellsWithSameItem)
            {
                countToAdd = FillCell(item, countToAdd, cell);
                
                if (countToAdd == 0)
                {
                    break;
                }
            }

            return countToAdd;
        }

        private static int FillCell(ItemSO item, int countToAdd, InventoryCell cell)
        {
            if (cell.Item == null)
            {
                cell.Item = item;
            }

            if (cell.AvailableCount < countToAdd)
            {
                countToAdd -= cell.AvailableCount;
                cell.Count += cell.AvailableCount;
            }
            else
            {
                cell.Count += countToAdd;
                countToAdd = 0;
            }

            return countToAdd;
        }

        public int ItemCount(ItemSO item)
        {
            return Cells.Where(c => c.Item == item).Sum(i => i.Count);
        }

        public void MoveItem(int startIndex, int destinationIndex)
        {
            (Cells[startIndex], Cells[destinationIndex]) = (Cells[destinationIndex], Cells[startIndex]);
        }

        public int RemoveItem(ItemSO item, int countToRemove)
        {
            var cellsWithItem = GetCellsWithSameItem(item);

            foreach (var cell in cellsWithItem)
            {
                countToRemove = RemoveFromCell(countToRemove, cell);

                if (countToRemove == 0)
                {
                    break;
                }
            }

            return countToRemove;
        }

        private static int RemoveFromCell(int countToRemove, InventoryCell cell)
        {
            if (cell.Count < countToRemove)
            {
                countToRemove -= cell.Count;
                cell.Item = null;
                cell.Count = 0;
            }
            else
            {
                cell.Count -= countToRemove;
                countToRemove = 0;
            }

            return countToRemove;
        }

        public int RemoveItem(int cellIndex, int countToRemove)
        {
            countToRemove = RemoveFromCell(countToRemove, Cells[cellIndex]);

            return countToRemove;
        }

        private IEnumerable<InventoryCell> GetFreeCells()
        {
            return Cells.Where(t => t.Item == null).ToList();
        }

        private int FindCellWithSameItem(ItemSO itemSo)
        {
            for (var i = 0; i < Cells.Length; i++)
            {
                if (Cells[i].Item == itemSo)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}