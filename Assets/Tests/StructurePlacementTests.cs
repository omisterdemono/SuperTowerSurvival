using System.Collections;
using Inventory;
using Inventory.Tests;
using NUnit.Framework;
using StructurePlacement;
using Structures;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SuperTowerSurvival.Tests
{
    public class StructurePlacementTests
    {
        private Scene _testScene;

        [SetUp]
        public void SetUpTests()
        {
            SceneManager.LoadSceneAsync("IntegrationTestingMultiplayer");
        }
    
        [TearDown]
        public void TearDownTests()
        {
            SceneManager.UnloadSceneAsync("IntegrationTestingMultiplayer");
        }

        [UnityTest]
        public IEnumerator Always_01_PlaceTest()
        {
            yield return new WaitForSeconds(1.0f);
            
            var inventoryTester = GameObject.FindObjectOfType<InventoryTester>();
            var character = GameObject.FindObjectOfType<Character>();
            var playerInventory = character.GetComponent<PlayerInventory>();
            var structurePlacer = character.GetComponent<StructurePlacer>();
        
            inventoryTester.AddItem();
            character.SelectInstrumentById("build_hammer");

            var defenceGunItem = playerInventory.ItemDatabase.GetItemSOById("defence_gun");
            structurePlacer.SelectStructure(defenceGunItem, null);
        
            Assert.IsTrue(structurePlacer.CurrentStructureId.Equals("defence_gun"));

            structurePlacer.PlaceStructure(new Vector3(2.5f, 0.5f, 0));
            var placedStructure = Object.FindObjectOfType<DefenceStructure>();
        
            Assert.IsNotNull(placedStructure);
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator Never_02_PlaceTest()
        {
            yield return new WaitForSeconds(1.0f);
        
            var inventoryTester = GameObject.FindObjectOfType<InventoryTester>();
            var character = GameObject.FindObjectOfType<Character>();
            var playerInventory = character.GetComponent<PlayerInventory>();
            var structurePlacer = character.GetComponent<StructurePlacer>();
        
            inventoryTester.AddItem();
            character.SelectInstrumentById("build_hammer");

            var defenceGunItem = playerInventory.ItemDatabase.GetItemSOById("defence_gun");
            structurePlacer.SelectStructure(defenceGunItem, null);

            structurePlacer.TempStructure.transform.position = new Vector3(5f, 0.5f, 0);
            
            Assert.IsFalse(structurePlacer.StructureCanBePlaced);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Never_03_PlaceOnOtherStructureTest()
        {
            yield return new WaitForSeconds(1.0f);
        
            var inventoryTester = GameObject.FindObjectOfType<InventoryTester>();
            var character = GameObject.FindObjectOfType<Character>();
            var playerInventory = character.GetComponent<PlayerInventory>();
            var structurePlacer = character.GetComponent<StructurePlacer>();
        
            inventoryTester.AddItem();
            character.SelectInstrumentById("build_hammer");

            var defenceGunItem = playerInventory.ItemDatabase.GetItemSOById("defence_gun");
            structurePlacer.SelectStructure(defenceGunItem, null);
            
            structurePlacer.PlaceStructure(new Vector3(2.5f, 0.5f, 0));
            
            yield return new WaitForSeconds(0.1f);

            structurePlacer.TempStructure.transform.position = new Vector3(2.5f, 0.5f, 0);
            
            Assert.IsFalse(structurePlacer.StructureCanBePlaced);
            yield return null;
        }
    }
}
