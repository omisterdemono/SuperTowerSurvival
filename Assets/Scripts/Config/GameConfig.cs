using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public static class GameConfig
    {
        public static readonly int InventoryCellsCount = 18;
        public static readonly int HotbarCellsCount = 7;
        public static readonly int EquipCellsCount = 3;
        public static readonly List<KeyCode> ActiveSkillsKeyCodes = new() { KeyCode.Q, KeyCode.E, KeyCode.F, KeyCode.G };
        public static readonly List<KeyCode> HotbarKeyCodes = new() { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7 };
        public static readonly KeyCode OpenInventoryKeyCode = KeyCode.I;
        public static readonly KeyCode OpenCraftingKeyCode = KeyCode.C;
        public static readonly KeyCode DropItemKeyCode = KeyCode.G;
        public static readonly KeyCode CancelPlacementKeyCode = KeyCode.Escape;
    }
}