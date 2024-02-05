using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Models;
using JetBrains.Annotations;

namespace Inventory
{
    public class Inventory
    {
        public InventoryCell[] Cells { get; private set; }
        public bool IsEmpty => Cells.Any(c => c.Item == null);
        public bool IsFull => Cells.All(c => c.IsFull);

        public Inventory(int count)
        {
            Cells = new InventoryCell[count];

            for (var i = 0; i < count; i++)
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
        
        public int TryAddToCell(int cellIndex, int countToAdd)
        {
            var itemSo = Cells[cellIndex].Item;
            if (itemSo != null)
                countToAdd = FillCell(itemSo, countToAdd, Cells[cellIndex]);

            return countToAdd;
        }

        public int TryAddToCell([NotNull] ItemSO item, int cellIndex, int countToAdd)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            countToAdd = FillCell(item, countToAdd, Cells[cellIndex]);

            return countToAdd;
        }

        private IEnumerable<InventoryCell> GetCellsWithSameItem(ItemSO item)
        {
            return Cells.Where(c => c.Item == item);
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

            cell.Modified.Invoke(cell);
            
            return countToAdd;
        }

        public int ItemCount(ItemSO item)
        {
            return Cells.Where(c => c.Item == item).Sum(i => i.Count);
        }

        public void MoveItem(int startIndex, int destinationIndex)
        {
            var startCell = Cells[startIndex];
            var destinationCell = Cells[destinationIndex];
            var (item, count) = (startCell.Item, startCell.Count);
            
            (startCell.Item, startCell.Count) = (destinationCell.Item, destinationCell.Count);
            (destinationCell.Item, destinationCell.Count) = (item, count);
            
            startCell.Modified.Invoke(startCell);
            destinationCell.Modified.Invoke(destinationCell);
        }

        public int TryRemoveItem(ItemSO item, int countToRemove)
        {
            var cellsWithItem = GetCellsWithSameItem(item);

            foreach (var cell in cellsWithItem)
            {
                countToRemove = TryRemoveFromCell(cell, countToRemove);

                if (countToRemove == 0)
                {
                    break;
                }
            }

            return countToRemove;
        }

        public int TryRemoveFromCell(InventoryCell cell, int countToRemove)
        {
            if (cell.Count <= countToRemove)
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
            cell.Modified.Invoke(cell);

            return countToRemove;
        }

        public int TryRemoveFromCell(int cellIndex, int countToRemove)
        {
            countToRemove = TryRemoveFromCell(Cells[cellIndex], countToRemove);

            return countToRemove;
        }

        private IEnumerable<InventoryCell> GetFreeCells()
        {
            return Cells.Where(t => t.Item == null).ToList();
        }
    }
}