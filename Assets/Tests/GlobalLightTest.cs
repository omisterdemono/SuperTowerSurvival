using System.Collections;
using System.Collections.Generic;
using Components;
using NUnit.Framework;
using Structures;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GlobalLightTest
{
    [UnityTest]
    public IEnumerator Always_01_GlobalLightPassedMidday()
    {
        SceneManager.LoadSceneAsync("IT_globalLight");

        yield return new WaitForSeconds(1.0f);

        var light = GameObject.FindObjectOfType<WorldLight>();
        
        yield return new WaitForSeconds(10.0f);
        var actualHour = light.GetHour();

        Assert.IsTrue(actualHour > 12);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Always_01_GlobalLightIsNight()
    {
        SceneManager.LoadSceneAsync("IT_globalLight");

        yield return new WaitForSeconds(1.0f);

        var light = GameObject.FindObjectOfType<WorldLight>();

        yield return new WaitForSeconds(9.0f);  

        Assert.IsTrue(light.isNight);

        yield return null;
    }
}
