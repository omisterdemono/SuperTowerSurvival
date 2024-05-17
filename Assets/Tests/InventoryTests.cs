using System.Collections;
using Inventory;
using Inventory.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SuperTowerSurvival.Tests
{
    public class InventoryTests
    {
        [UnityTest]
        public IEnumerator Always_01_ItemsByDefaultTest()
        {
            yield return SceneManager.LoadSceneAsync("IntegrationTestingMultiplayer");
            yield return new WaitForSeconds(1.0f);

            var character = GameObject.FindObjectOfType<Character>();
            var playerInventory = character.GetComponent<PlayerInventory>();
            
            Assert.IsTrue(playerInventory.Inventory.ItemCount("build_hammer") == 1);
        }
        
        [UnityTest]
        public IEnumerator Always_02_AddItemsTest()
        {
            if (!SceneManager.GetActiveScene().name.Equals("IntegrationTestingMultiplayer"))
            {
                yield return SceneManager.LoadSceneAsync("IntegrationTestingMultiplayer");
                yield return new WaitForSeconds(1.0f);  
            }

            var inventoryTester = GameObject.FindObjectOfType<InventoryTester>();
            var character = GameObject.FindObjectOfType<Character>();
            var playerInventory = character.GetComponent<PlayerInventory>();

            inventoryTester.AddItem();

            Assert.IsTrue(playerInventory.Inventory.ItemCount("defence_gun") == 4);
        }

        [UnityTest]
        public IEnumerator Always_03_DropItemsTest()
        {
            if (!SceneManager.GetActiveScene().name.Equals("IntegrationTestingMultiplayer"))
            {
                yield return SceneManager.LoadSceneAsync("IntegrationTestingMultiplayer");
                yield return new WaitForSeconds(1.0f);  
            }
            var character = GameObject.FindObjectOfType<Character>();
            var playerInventory = character.GetComponent<PlayerInventory>();
            
            var buildHammerItem = playerInventory.ItemDatabase.GetItemSOById("build_hammer");

            playerInventory.Inventory.TryRemoveItem(buildHammerItem, 1);

            Assert.IsTrue(playerInventory.Inventory.ItemCount("build_hammer") == 0);
        }
    }
}