using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Wave
{
    public int waveID;
    public int enemiesPerSpawner;

    public void SetParams(int waveID, int enemiesNumber)
    {
        this.waveID = waveID;
        this.enemiesPerSpawner = enemiesNumber;
    }

    public void SetParamsWithKoef(Wave baseWave, float koef)
    {
        this.waveID = baseWave.waveID + 1;
        this.enemiesPerSpawner = Mathf.RoundToInt(baseWave.enemiesPerSpawner * koef);
    }

    public void SetParamsWithAdd(Wave baseWave, int add)
    {
        this.waveID = baseWave.waveID + 1;
        this.enemiesPerSpawner = baseWave.enemiesPerSpawner + add;
    }
}
