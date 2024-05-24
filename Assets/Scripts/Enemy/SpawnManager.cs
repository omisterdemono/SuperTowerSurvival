using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private int _spawnInnerRadius = 10;
    [SerializeField] private int _spawnOuterRadius = 20;
    [SerializeField] private int _maxSpawnedNumber = 5;
    [SerializeField] private float _cooldownSeconds;
    [SerializeField] private List<GameObject> _enemyTypesPrefabs = new List<GameObject>();
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private List<float> _enemyTypesPrefabsWeights = new List<float>();

    [SyncVar] public int _actualSpawnedNumber = 0;

    private float _timeToNextSpawn = 0;
    private WorldLight _worldLight;
    private List<Character> players;
    //bool isBossExisting;
    public bool isBossExisting;

    void Start()
    {
        isBossExisting = false;
        if (_enemyTypesPrefabs.Count() != _enemyTypesPrefabsWeights.Count())
        {
            throw new System.Exception("Invalid key-value");
        }
        _worldLight = FindObjectOfType<WorldLight>();
        players = FindObjectsOfType<Character>().ToList();
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
        if (!isBossExisting)
        {
            SpawnBoss();
        }
    }

    private Vector3 GetRandomPlayerPosition() => players[Random.Range(0, players.Count)].transform.position;

    private Vector3 GetSpawnPosition()
    {
        
        var playerPos = GetRandomPlayerPosition();
        
        while (true)
        {
            float distance = Random.Range(_spawnInnerRadius, _spawnOuterRadius);

            float angle = Random.value * 2 * Mathf.PI;

            float posX = playerPos.x + distance * Mathf.Cos(angle);
            float posY = playerPos.y + distance * Mathf.Sin(angle);
            
            var pos = new Vector3(posX, posY);

            return pos;

            //if (pos.x < 0)
            //{
            //    return pos;
            //}
        }
    }

    [Command(requiresAuthority = false)]
    public void SpawnEnemy()
    {
        if (_timeToNextSpawn != 0)
        {
            return;
        }

        GameObject type = ChoseType();
        if (type == null)
        {
            throw new System.Exception("Couldnt find a type in list");
        }

        _actualSpawnedNumber++;
        //GameObject newEnemy = Instantiate(type, new Vector3(posX, posY), Quaternion.identity);
        GameObject newEnemy = Instantiate(type, GetSpawnPosition(), Quaternion.identity);

        _timeToNextSpawn = _cooldownSeconds;
        NetworkServer.Spawn(newEnemy);
    }

    [Command(requiresAuthority = false)]
    public void SpawnBoss()
    {
        var pos = GetSpawnPosition();
        GameObject newBoss = Instantiate(_bossPrefab, GetSpawnPosition(), Quaternion.identity);

        NetworkServer.Spawn(newBoss);
        isBossExisting = true;
    }
}
