using JetBrains.Annotations;
using Mirror;
using Mirror.Examples.Tanks;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SpawnerComponent : NetworkBehaviour
{
    [SerializeField] private int _spawnRadius = 5;
    [SerializeField] private int _maxSpawnedNumber = 5;
    [SerializeField] private float _cooldownSeconds;
    [SerializeField] private List<GameObject> _enemyTypesPrefabs = new List<GameObject>();
    [SerializeField] private List<float> _enemyTypesPrefabsWeights = new List<float>();
    
    [SyncVar] public int _actualSpawnedNumber = 0;

    private float _timeToNextSpawn = 0;
    private WorldLight _worldLight;

    void Start()
    {
        if (_enemyTypesPrefabs.Count() != _enemyTypesPrefabsWeights.Count())
        {
            throw new System.Exception("Invalid key-value");
        }
        _worldLight = FindObjectOfType<WorldLight>();
    }

    public void UpdateSpawnerParams(int newMaxSpawnedNumber)
    {
        _actualSpawnedNumber = 0;
        _maxSpawnedNumber = newMaxSpawnedNumber;
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

    void Update()
    {
        HandleSpawnRate();
        Spawn();
    }

    [Server]
    private void HandleSpawnRate()
    {
        if (_timeToNextSpawn == 0)
        {
            return;
        }

        if (_timeToNextSpawn < 0)
        {
            _timeToNextSpawn = 0;
            return;
        }

        _timeToNextSpawn -= Time.deltaTime;
    }

    public void Spawn()
    {
        if (_actualSpawnedNumber < _maxSpawnedNumber && _worldLight.isNight)
        {
            SpawnEnemy();
        }
    }

    [Command(requiresAuthority = false)]
    public void SpawnEnemy()
    {
        if (_timeToNextSpawn != 0)
        {
            return;
        }

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

        _timeToNextSpawn = _cooldownSeconds;
        NetworkServer.Spawn(newEnemy);
    }
}
