using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public static class GameConfig
    {
        public static readonly int InventoryCellsCount = 18;
        public static readonly int HotbarCellsCount = 7;
        public static readonly List<KeyCode> ActiveSkillsKeyCodes = new() { KeyCode.Q, KeyCode.E, KeyCode.F, KeyCode.G };
    }
}