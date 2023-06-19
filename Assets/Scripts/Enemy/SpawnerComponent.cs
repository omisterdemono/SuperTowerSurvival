using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerComponent : NetworkBehaviour
{
    [SerializeField] private int _spawnRadius = 5;
    [SerializeField] private int _maxSpawnedNumber = 7;
    [SerializeField] private List<GameObject> _enemyTypesPrefabs = new List<GameObject>();
    [SerializeField] private List<float> _enemyTypesPrefabsWeights = new List<float>();
    
    [SyncVar] private int _actualSpawnedNumber = 0;

    private float _spawnTimeSeconds;
    private bool _isSpawnable;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemyTypesPrefabs.Count() != _enemyTypesPrefabsWeights.Count())
        {
            throw new System.Exception("Invalid key-value");
        }
    }

    //[Command(requiresAuthority = false)]
    void Spawn()
    {
        float radius = Random.value * _spawnRadius;
        float angle = Random.value * 2 * Mathf.PI;

        float posX = transform.position.x + radius * Mathf.Cos(angle);
        float posY = transform.position.y + radius * Mathf.Sin(angle);

        GameObject type = ChoseType();
        if (type == null)
        {
            throw new System.Exception("Couldnt find a type in list");
        }

        _actualSpawnedNumber++;
        GameObject newEnemy = Instantiate(type, new Vector3(posX, posY), Quaternion.identity);
        //NetworkServer.Spawn(newEnemy);
    }

    GameObject ChoseType()
    {
        float totalWeight = _enemyTypesPrefabsWeights.Sum();

        float randomNum = Random.value * totalWeight;

        for (int i = 0; i < _enemyTypesPrefabs.Count(); i++)
        {
            var enemyWeight = _enemyTypesPrefabsWeights[i];
            if (randomNum < enemyWeight)
            {
                return _enemyTypesPrefabs[i];
            }
            randomNum -= enemyWeight;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        while (_actualSpawnedNumber < _maxSpawnedNumber)
        {
            Spawn();
        }
    }
}
