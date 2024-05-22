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
        _worldCycle = GetComponent<WorldLight>();
        _spawners = FindObjectsOfType<SpawnManager>().ToList();
        Debug.Log("[Game init] Waves Manager Awake");
    }

    void Start()
    {
        _waves = new List<Wave>();
        var firstWave = new Wave();
        firstWave.SetParams(1, initEnemiesNumber);
        _waves.Add(firstWave);
        GenerateWaves();
        UpdateSpawnersParams();
        _worldCycle.OnIsNightChanged += UpdateSpawnersParams;
        
        Debug.Log("[Game init] Waves Manager Start");
    }

    public void OnDestroy()
    {
        _worldCycle.OnIsNightChanged -= UpdateSpawnersParams;
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
        Wave currentWave = _waves.FirstOrDefault(w => w.waveID == currentDay);

        if (currentDay == 0)
        {
            return;
        }
        
        if (currentDay > _waves.Last()?.waveID)
        {
            currentWave = _waves.Last();
        }
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
