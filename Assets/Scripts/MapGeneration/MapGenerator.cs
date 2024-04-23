using UnityEngine;
using DefaultNamespace;
using UnityEngine.Serialization;

public class MapGenerator : MonoBehaviour
{
    public enum GenerationMode
    {
        Simple,
        Island
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

    [FormerlySerializedAs("regions")] [SerializeField] private Biome[] _regions;
    
    [Header("Island settings")] 
    [SerializeField] private float _islandRadius;
    [SerializeField] private float _islandRadiusOffset;

    private float[,] _landMap;
    private float[,] _temperatureMap;
    private MapDisplay _mapDisplay;

    private void Start()
    {
        _mapDisplay = FindObjectOfType<MapDisplay>();
    }

    public void GenerateMap()
    {
        _landMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, seed, _noiseScale, _octaves, persistance,
            lacunarity, offset);

        var landTilemap = FindObjectOfType<MapDisplay>().LandTilemap;

        if (_generationMode == GenerationMode.Island)
        {
            CutIsland(_landMap);
        }

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float currentHeight = _landMap[x, y];

                for (int i = 0; i < _regions.Length; i++)
                {
                    if (currentHeight <= _regions[i].RequiredHeight)
                    {
                        landTilemap.SetTile(new Vector3Int(x - _mapWidth / 2, y - _mapHeight / 2), _regions[i].Tile);
                        break;
                    }
                }
            }
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
        FindObjectOfType<MapDisplay>().LandTilemap.ClearAllTiles();
    }
}