using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] private int wavesNumber = 15;
    [SerializeField] private int initEnemiesNumber = 3;
    [SerializeField] private float multiplyKoef = 1.2f;

    private WorldLight _worldCycle;
    private List<SpawnManager> _spawners;
    private List<Wave> _waves;

    private void Awake()
    {
    }

    void Start()
    {
        _worldCycle = GetComponent<WorldLight>();
        _spawners = FindObjectsOfType<SpawnManager>().ToList();
        _waves = new List<Wave>();
        var firstWave = new Wave();
        firstWave.SetParams(1, initEnemiesNumber);
        _waves.Add(firstWave);
        GenerateWaves();
            //Debug.Log($"[!] worldCycle before cycle {_worldCycle}");
            //while (_worldCycle == null)
            //{
            //    _worldCycle = GetComponent<WorldLight>();
            //}
            //Debug.Log($"[!] _worldCycle after cycle {_worldCycle}");
        _worldCycle.OnIsNightChanged += UpdateSpawnersParams;
        //UpdateSpawnersParams();
    }

    public void OnDestroy()
    {
        //Debug.Log($"[!] _worldCycle ENDstate {_worldCycle}");
        //_worldCycle.OnIsNightChanged -= UpdateSpawnersParams;
    }

    void Update()
    {

    }

    private void GenerateWaves()
    {
        for (int id = 1; id < wavesNumber; id++)
        {
            var nextWave = new Wave();
            //nextWave.SetParamsWithAdd(_waves[id - 1], 2);
            nextWave.SetParamsWithAdd(_waves[id - 1], 2);
            _waves.Add(nextWave);
        }
    }

    private void UpdateSpawnersParams()
    {
        int currentDay = _worldCycle.GetDay();
        //if(_waves == null)
        //{
        //    _wa
        //}
        Wave currentWave = _waves.FirstOrDefault(w => w.waveID == currentDay);
        if (currentDay > _waves.Last()?.waveID)
        {
            currentWave = _waves.Last();
        }
            //Debug.Log($"[!] spawners before cycle {_spawners}");
            //while (_spawners == null)
            //{
            //    _spawners = FindObjectsOfType<SpawnManager>().ToList();
            //}
            //Debug.Log($"[!] spawners after cycle {_spawners}");
        foreach (var spawner in _spawners)
        {
            spawner.UpdateSpawnerParams(currentWave.enemiesPerSpawner);
        }
    }

    public int GetWavesNumber()
    {
        return wavesNumber;
    }
}
