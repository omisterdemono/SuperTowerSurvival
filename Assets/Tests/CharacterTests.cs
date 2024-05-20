using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SuperTowerSurvival.Tests
{
    public class CharacterTests
    {
        [UnityTest]
        public IEnumerator Always_01_SelectInstrumentTest()
        {
            yield return SceneManager.LoadSceneAsync("IntegrationTestingMultiplayer");
            yield return new WaitForSeconds(1.0f);

            var character = GameObject.FindObjectOfType<Character>();

            character.SelectInstrumentById("build_hammer");

            var buildHammer = character.gameObject.GetComponentInChildren<BuildHammer>();
            Assert.IsTrue(buildHammer.gameObject.activeSelf);
            yield return SceneManager.UnloadSceneAsync("IntegrationTestingMultiplayer");
        }
    }
}