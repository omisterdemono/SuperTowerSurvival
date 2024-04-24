using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Data", menuName = "Gen/Biome", order = 1)]
    public class Biome : ScriptableObject
    {
        public string Name;
        public float MinHeight;
        [FormerlySerializedAs("RequiredHeight")] public float MaxHeight;
        public float RequiredTemperature;
        public Color Color;
        public TileBase Tile;
    }
}