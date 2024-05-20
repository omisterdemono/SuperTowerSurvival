using System.Collections;
using Inventory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SuperTowerSurvival.Tests
{
    public class DefenceStructuresTests
    {
        private Scene _testScene;
        
        [UnityTest]
        public IEnumerator Always_01_EnemyExistsInRange()
        {
            SceneManager.LoadSceneAsync("IntegrationTestingMultiplayer", LoadSceneMode.Single);
            
            yield return new WaitForSeconds(1.0f);
            var enemy = GameObject.FindObjectOfType<Enemy>();   
            Assert.IsTrue(enemy is not null);

            SceneManager.UnloadSceneAsync("IntegrationTestingMultiplayer", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Always_02_AttacksEnemyInRange()
        {
            SceneManager.LoadSceneAsync("IntegrationTestingMultiplayer", LoadSceneMode.Single);

            yield return new WaitForSeconds(3.0f);
            var enemy = GameObject.FindObjectOfType<Enemy>();
            Assert.IsTrue(enemy is null);
            
            SceneManager.UnloadSceneAsync("IntegrationTestingMultiplayer", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            yield return null;
        }
    }
}
