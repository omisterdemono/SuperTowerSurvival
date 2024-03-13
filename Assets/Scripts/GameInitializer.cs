using System.Collections.Generic;
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

            for (var index = 0; index < activeSkills.Count; index++)
            {
                var activeSkill = activeSkills[index];
                var skillButton = Instantiate(_skillButton, skillHolder.transform, true);
                activeSkill.SkillButton = skillButton;
                
                skillButton.SkillIcon.sprite = activeSkill.SkillAttributes.SkillIcon;
                skillButton.ActivateButtonText.text = Config.GameConfig.ActiveSkillsKeyCodes[index].ToString();
                
                skillButtons.Add(skillButton);
            }

            return skillButtons;
        }
    }
}