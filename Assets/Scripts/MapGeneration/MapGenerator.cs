using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DefaultNamespace;
using Infrastructure;
using Mirror;
using Unity.Mathematics;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class MapGenerator : NetworkBehaviour
{
    public enum GenerationMode
    {
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

    [SerializeField] private Biome[] _regions;

    [Header("Island settings")] [SerializeField]
    private float _islandRadius;

    [SerializeField] private int _mainHallRequiredSpace;
    [SerializeField] private string _townHallSpawnBiome = "Plains";
    [SerializeField] private float _islandRadiusOffset;
    [SerializeField] private Tilemap _waterTilemap;

    [FormerlySerializedAs("_landTilemap")] [SerializeField]
    private Tilemap _mainTilemap;

    [Header("Resource settings")] [SerializeField]
    private MapGenerator _landMapGenerator;

    [SerializeField] private ObtainableProps[] _obtainablesData;

    public float[,] NoiseMap { get; private set; }

    private float[,] _temperatureMap;
    private float[] _randomColourValues = { 1, 0.99f, 0.98f, 0.97f, 0.96f, 0.95f, 0.94f };
    private MapDisplay _mapDisplay;
    private bool _townHallIsPlaced;

    public void GenerateMap(int seedFromPlayer)
    {
        if (!isServer)
        {
            return;
        }

        _mainTilemap.ClearAllTiles();

        switch (_generationMode)
        {
            case GenerationMode.Island:
                NoiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, seedFromPlayer, _noiseScale, _octaves,
                    persistance,
                    lacunarity, offset);
                CutIsland(NoiseMap);

                System.Random random = new(seedFromPlayer);

                for (int y = 0; y < _mapHeight; y++)
                {
                    for (int x = 0; x < _mapWidth; x++)
                    {
                        float currentHeight = NoiseMap[x, y];

                        for (int i = 0; i < _regions.Length; i++)
                        {
                            if (currentHeight > _regions[i].MinHeight && currentHeight <= _regions[i].MaxHeight)
                            {
                                var tilePosition = new Vector3Int(x - _mapWidth / 2, y - _mapHeight / 2);

                                if (IsWaterTile(i))
                                {
                                    PlaceWaterTile(tilePosition, i);
                                    break;
                                }

                                PlaceTile(tilePosition, i);

                                if (!_townHallIsPlaced && _regions[i].Name.Equals(_townHallSpawnBiome))
                                {
                                    TryPlaceMainHall(tilePosition, x, y, i);
                                }

                                //todo fix colour
                                // var index = random.Next(0, _randomColourValues.Length - 1);
                                // var randomNumber = _randomColourValues[index];
                                //
                                // _mainTilemap.SetTileFlags(tilePosition, TileFlags.None);
                                // _mainTilemap.SetColor(tilePosition,
                                //     new Color(randomNumber, randomNumber, randomNumber));
                                break;
                            }
                        }
                    }
                }

                break;
            case GenerationMode.Resources:
                NoiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, seedFromPlayer, _noiseScale, _octaves,
                    persistance,
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
                                    Quaternion.identity, _mainTilemap.transform);

                                NetworkServer.Spawn(obtainable.gameObject);
                                _placedObtainables.Add(obtainable.transform);
                                break;
                            }
                        }
                    }
                }

                break;
        }
    }

    private void TryPlaceMainHall(Vector3Int tilePosition, int x, int y, int i)
    {
        for (int j = x - _mainHallRequiredSpace; j < x + _mainHallRequiredSpace; j++)
        {
            for (int k = y - _mainHallRequiredSpace; k < y + _mainHallRequiredSpace; k++)
            {
                if(NoiseMap[j,k] < _regions[i].MinHeight || NoiseMap[j,k] > _regions[i].MaxHeight)
                {
                    return;
                }
            }
        }

        PlaceBase(tilePosition);
        
        var spawnPoints = GameObject.FindGameObjectWithTag("SpawnPoints").transform;
        
        var characters = FindObjectsOfType<Character>();
        for (int j = 0; j < characters.Length; j++)
        {
            var child = spawnPoints.GetChild(j);
            MovePlayer(characters[j].netId, child.position);
        }

        _townHallIsPlaced = true;
    }

    [ClientRpc]
    private void PlaceBase(Vector3Int tilePosition)
    {
        var mainHall = GameObject.FindGameObjectWithTag("MainHall").transform;
        mainHall.position = tilePosition;
    }

    [ClientRpc]
    private void MovePlayer(uint netId, Vector3 newPosition)
    {
        var character = FindObjectsOfType<Character>().First(c => c.isOwned);

        if (character.netId == netId)
        {
            character.transform.SetPositionAndRotation(newPosition, quaternion.identity);
        }
    }

    [ClientRpc]
    private void PlaceWaterTile(Vector3Int tilePosition, int regionIndex)
    {
        _waterTilemap.SetTile(tilePosition, _regions[regionIndex].Tile);
    }

    [ClientRpc]
    private void PlaceTile(Vector3Int tilePosition, int regionIndex)
    {
        _mainTilemap.SetTile(tilePosition, _regions[regionIndex].Tile);
    }

    private bool IsWaterTile(int i)
    {
        return _regions[i].Name is "Sea" or "Ocean";
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