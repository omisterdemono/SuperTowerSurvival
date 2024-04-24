﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DefaultNamespace;
using Infrastructure;
using UnityEngine.Serialization;

public class MapGenerator : MonoBehaviour
{
    public enum GenerationMode
    {
        Simple,
        Island,
        Resources
    };

    [SerializeField] private GenerationMode _generationMode;

    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    [SerializeField] private float _noiseScale;

    [SerializeField] private int _octaves;
    [SerializeField] [Range(0, 1)] private float persistance;
    [SerializeField] private float lacunarity;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    public bool AutoUpdate;

    [FormerlySerializedAs("regions")] [SerializeField]
    private Biome[] _regions;

    [Header("Island settings")] [SerializeField]
    private float _islandRadius;

    [SerializeField] private float _islandRadiusOffset;

    [FormerlySerializedAs("_mapGenerator")] [Header("Resource settings")] [SerializeField]
    private MapGenerator _landMapGenerator;

    [FormerlySerializedAs("_obtainables")] [SerializeField]
    private ObtainableProps[] _obtainablesData;

    public float[,] NoiseMap { get; private set; }

    private float[,] _temperatureMap;
    private MapDisplay _mapDisplay;

    private void Awake()
    {
    }

    public void GenerateMap()
    {
        var landTilemap = GetComponent<MapDisplay>().Tilemap;
        landTilemap.ClearAllTiles();

        switch (_generationMode)
        {
            case GenerationMode.Island:
                NoiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, seed, _noiseScale, _octaves, persistance,
                    lacunarity, offset);
                CutIsland(NoiseMap);

                for (int y = 0; y < _mapHeight; y++)
                {
                    for (int x = 0; x < _mapWidth; x++)
                    {
                        float currentHeight = NoiseMap[x, y];

                        for (int i = 0; i < _regions.Length; i++)
                        {
                            if (currentHeight > _regions[i].MinHeight && currentHeight <= _regions[i].MaxHeight)
                            {
                                landTilemap.SetTile(new Vector3Int(x - _mapWidth / 2, y - _mapHeight / 2),
                                    _regions[i].Tile);
                                break;
                            }
                        }
                    }
                }

                break;
            case GenerationMode.Resources:
                NoiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, seed, _noiseScale, _octaves, persistance,
                    lacunarity, offset);
                _landMapGenerator = FindObjectsOfType<MapGenerator>().First(o => o.name.Equals("Island Map Generator"));

                var _placedObtainables = new List<Transform>();

                for (int y = 0; y < _mapHeight; y++)
                {
                    for (int x = 0; x < _mapWidth; x++)
                    {
                        float heightOnLand = _landMapGenerator.NoiseMap[x, y];

                        for (int i = 0; i < _obtainablesData.Length; i++)
                        {
                            if (!(heightOnLand >= _obtainablesData[i].MinHeight) ||
                                !(heightOnLand <= _obtainablesData[i].MaxHeight))
                            {
                                continue;
                            }

                            float heightForResource = NoiseMap[x, y];
                            if (!(heightForResource >= _obtainablesData[i].MinHeight) ||
                                !(heightForResource <= _obtainablesData[i].MaxHeight))
                            {
                                continue;
                            }

                            var expectedPosition = new Vector3(x - _mapWidth / 2, y - _mapHeight / 2, 0);
                            var (closest, distanceToClosest) = _placedObtainables.Closest(expectedPosition);
                            if (closest is null || distanceToClosest >= _obtainablesData[i].DistanceThreshold)
                            {
                                var obtainable = Instantiate(_obtainablesData[i].ObtainablePrefab,
                                    expectedPosition,
                                    Quaternion.identity, landTilemap.transform);
                                _placedObtainables.Add(obtainable.transform);
                                break;
                            }
                        }
                    }
                }

                break;
        }
    }

    private void CutIsland(float[,] noiseMap)
    {
        Vector2 centerPoint = new Vector2(_mapWidth / 2f, _mapHeight / 2f);
        Vector2 currentPoint = new Vector2();

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                currentPoint.x = x;
                currentPoint.y = y;

                var distanceFromCenterToPoint = Vector2.Distance(centerPoint, currentPoint);
                if (distanceFromCenterToPoint > _islandRadius)
                {
                    var multiplier = 1.0f - Mathf.InverseLerp(_islandRadius, _islandRadius + _islandRadiusOffset,
                        distanceFromCenterToPoint);
                    noiseMap[x, y] *= multiplier;
                }
            }
        }
    }

    private bool IsLandAround()
    {
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                //check if height of one of xy tiles is suitable for land
                //if true, then create collider
            }
        }

        return false;
    }

    private void OnValidate()
    {
        if (_mapWidth < 1)
        {
            _mapWidth = 1;
        }

        if (_mapHeight < 1)
        {
            _mapHeight = 1;
        }

        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (_octaves < 0)
        {
            _octaves = 0;
        }
    }

    public void ClearMap()
    {
        FindObjectOfType<MapDisplay>().Tilemap.ClearAllTiles();
    }
}