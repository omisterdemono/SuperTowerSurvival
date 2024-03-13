using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.UI;
using Inventory.UI;
using UnityEngine;

namespace Infrastructure
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private InventoryUI _inventoryUIPrefab;
        [SerializeField] private CraftingUI _craftingUIPrefab;
        [SerializeField] private SkillButton _skillButton;
        [SerializeField] private HotbarInventoryCellUI _hotbarInventoryCell;

        public InventoryUI InitializeInventoryUI()
        {
            var canvas = FindObjectOfType<Canvas>();
            return Instantiate(_inventoryUIPrefab, canvas.transform);
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

        public void InitializeHotbar()
        {
            var hotbar = GameObject.FindGameObjectWithTag("ItemHolder");

            for (int i = 0; i < Config.GameConfig.HotbarKeyCodes.Count; i++)
            {
                var hotbarCell = Instantiate(_hotbarInventoryCell, hotbar.transform);
                var keycode = Config.GameConfig.HotbarKeyCodes[i].ToString();
                hotbarCell.ActivateButtonHintText.text = keycode.Last().ToString();

                if (i == 0)
                {
                    hotbarCell.Select(true);
                }
            }
        }
    }
}