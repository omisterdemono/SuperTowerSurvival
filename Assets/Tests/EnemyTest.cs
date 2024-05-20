using System.Collections;
using System.Collections.Generic;
using Inventory.Tests;
using Inventory;
using NUnit.Framework;
using StructurePlacement;
using Structures;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Components;

public class EnemyTest
{
    [UnityTest]
    public IEnumerator Always_01_EnemiesTargetMainHall()
    {
        SceneManager.LoadSceneAsync("IT_cannonChoose");

        yield return new WaitForSeconds(1.0f);

        var orc = GameObject.FindObjectOfType<Enemy>();
        var mainHall = GameObject.FindGameObjectWithTag("MainHall");
        var cannon = GameObject.FindObjectOfType<DefenceStructure>();

        var target = orc.Target;
        Assert.IsTrue(target == mainHall.transform);
        

        //Assert.IsTrue(target == cannon.transform);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Always_01_EnemiesTargetCannon()
    {
        SceneManager.LoadSceneAsync("IT_cannonChoose");

        yield return new WaitForSeconds(1.0f);

        var orc = GameObject.FindObjectOfType<Enemy>();
        //var mainHall = GameObject.FindGameObjectWithTag("MainHall");
        var cannon = GameObject.FindObjectOfType<DefenceStructure>();

        yield return new WaitForSeconds(3.0f);
        var target = orc.Target;
        Assert.IsTrue(target == cannon.transform);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Always_01_EnemiesTargetCannonAvoidWall()
    {
        SceneManager.LoadSceneAsync("IT_wallAvoid");

        yield return new WaitForSeconds(1.0f);

        var orc = GameObject.FindObjectOfType<Enemy>();
        //var mainHall = GameObject.FindGameObjectWithTag("MainHall");
        var cannon = GameObject.FindObjectOfType<DefenceStructure>();

        yield return new WaitForSeconds(3.0f);

        var target = orc.Target;
        
        Assert.IsTrue(target == cannon.transform);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Always_01_EnemiesTargetCannonThroughWall()
    {
        SceneManager.LoadSceneAsync("IT_wallBreak");

        yield return new WaitForSeconds(1.0f);

        var orc = GameObject.FindObjectOfType<Enemy>();
        orc.GetComponent<HealthComponent>().CurrentHealth = 100;
        //var mainHall = GameObject.FindGameObjectWithTag("MainHall");
        var cannon = GameObject.FindObjectOfType<DefenceStructure>();

        yield return new WaitForSeconds(3.0f);

        var target = orc.Target;

        Assert.IsTrue(target == cannon.transform);

        yield return null;
    }
}
