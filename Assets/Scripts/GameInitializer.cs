using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.UI;
using Inventory.UI;
using Mirror;
using UIScripts;
using UnityEngine;
using Random = System.Random;

namespace Infrastructure
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("UI")] [SerializeField] private SkillButton _skillButton;
        [SerializeField] private HotbarInventoryCellUI _hotbarInventoryCell;
        [SerializeField] private Canvas _waitCanvas;

        [Header("Map generation")] [SerializeField]
        private MapGenerator _landGenerator;

        [SerializeField] private MapGenerator _resourceGenerator;

        [Header("Testing")] [SerializeField] private bool _initNetworkManagerAutomatically;
        [SerializeField] private bool _generateMap;

        private void Start()
        {
            if (_initNetworkManagerAutomatically)
            {
                FindObjectOfType<NetworkManager>().StartHost();
            }

            if (_generateMap)
            {
                var random = new Random();
                GenerateMaps(random.Next(0, 10000));
            }
        }

        public InventoryUI InitializeInventoryUI()
        {
            return FindObjectOfType<InventoryUI>();
            ;
        }

        public EquipUI InitializeEquipUI()
        {
            return FindObjectOfType<EquipUI>();
        }

        public CraftingUI InitializeCraftingUI()
        {
            return FindObjectOfType<CraftingUI>();
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

        public IEnumerator GenerateMaps(int seed)
        {
            yield return StartCoroutine(_landGenerator.GenerateMap(seed));
            yield return StartCoroutine(_resourceGenerator.GenerateMap(seed));
        }

        public void HideWaitingCanvas()
        {
            FindObjectOfType<LoadingScreen>().gameObject.SetActive(false);
        }
    }
}