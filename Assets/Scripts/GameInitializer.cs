using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.UI;
using Inventory;
using Inventory.UI;
using UnityEngine;

namespace Infrastructure
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private InventoryUI _inventoryUIPrefab;
        [SerializeField] private EquipUI _equipUIPrefab;
        [SerializeField] private CraftingUI _craftingUIPrefab;
        [SerializeField] private SkillButton _skillButton;
        [SerializeField] private HotbarInventoryCellUI _hotbarInventoryCell;


        [SerializeField] private ItemDatabaseSO _itemDatabase;

        private void Awake()
        {
            // Cursor.visible = false;
        }

        public InventoryUI InitializeInventoryUI()
        {
            // var parent = GameObject.FindWithTag("InventoryHolderUI");
            // var inventoryUI = Instantiate(_inventoryUIPrefab, parent.transform, true);
            // inventoryUI.transform.SetSiblingIndex(1);
            
            return FindObjectOfType<InventoryUI>();;
        }

        public EquipUI InitializeEquipUI()
        {
            // var parent = GameObject.FindWithTag("InventoryHolderUI");
            // var equip = Instantiate(_equipUIPrefab, parent.transform, true);
            // equip.transform.SetSiblingIndex(0);
            return FindObjectOfType<EquipUI>();
        }

        public CraftingUI InitializeCraftingUI()
        {
            var canvas = FindObjectOfType<Canvas>();
            return Instantiate(_craftingUIPrefab, canvas.transform);
        }

        public List<SkillButton> InitializeSkillHolder(List<ActiveSkill> activeSkills)
        {
            var skillHolder = GameObject.FindGameObjectWithTag("SkillHolder");
            var skillButtons = new List<SkillButton>();

            for (var i = 0; i < activeSkills.Count; i++)
            {
                var activeSkill = activeSkills[i];
                var skillButton = Instantiate(_skillButton, skillHolder.transform, true);
                activeSkill.SkillButton = skillButton;

                skillButton.SkillIcon.sprite = activeSkill.SkillAttributes.SkillIcon;
                skillButton.ActivateButtonText.text = Config.GameConfig.ActiveSkillsKeyCodes[i].ToString();

                skillButtons.Add(skillButton);
            }

            return skillButtons;
        }

        public HotBar InitializeHotbar()
        {
            var hotbar = FindObjectOfType<HotBar>();

            for (int i = 0; i < Config.GameConfig.HotbarCellsCount; i++)
            {
                var hotbarCell = Instantiate(_hotbarInventoryCell, hotbar.transform);
                var keycode = Config.GameConfig.HotbarKeyCodes[i].ToString();
                hotbarCell.ActivateButtonHintText.text = keycode.Last().ToString();

                hotbar.HotbarCells.Add(hotbarCell);
            }

            hotbar.SelectCell(0);
            return hotbar;
        }
    }
}