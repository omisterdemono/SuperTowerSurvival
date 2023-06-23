using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] private int wavesNumber;
    [SerializeField] private int initEnemiesNumber = 10;
    [SerializeField] private float multiplyKoef = 1.2f;

    private WorldLight _worldCycle;
    private List<SpawnerComponent> _spawners;
    private List<Wave> _waves; 

    private void Awake()
    {
        _worldCycle = GetComponent<WorldLight>();
        _spawners = FindObjectsOfType<SpawnerComponent>().ToList();
    }

    void Start()
    {
        _waves = new List<Wave>();
        var firstWave = new Wave();
        firstWave.SetParams(1, initEnemiesNumber);
        _waves.Add(firstWave);
        GenerateWaves();
        _worldCycle.OnIsNightChanged += UpdateSpawnersParams;
    }

    public void OnDestroy()
    {
        _worldCycle.OnIsNightChanged -= UpdateSpawnersParams;
    }

    void Update()
    {
        //if (!_worldCycle.isNight)
        //{
        //    UpdateSpawnersParams();
        //}
    }

    private void GenerateWaves()
    {
        for (int id = 1; id < wavesNumber; id++)
        {
            var nextWave = new Wave();
            //nextWave.SetParamsWithKoef(_waves[id - 1], multiplyKoef);
            nextWave.SetParamsWithAdd(_waves[id - 1], 2);
            _waves.Add(nextWave);
        }
    }

    private void UpdateSpawnersParams()
    {
        int currentDay = _worldCycle.GetDay();
        Wave currentWave = _waves.FirstOrDefault(w => w.waveID == currentDay);
        foreach(var spawner in _spawners)
        {
            spawner.UpdateSpawnerParams(currentWave.enemiesPerSpawner);
        }
    }

    public int GetWavesNumber()
    {
        return wavesNumber;
    }
}
