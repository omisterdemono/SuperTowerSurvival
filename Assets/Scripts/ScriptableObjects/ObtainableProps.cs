using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Data", menuName = "Gen/Obtainable", order = 2)]
    public class ObtainableProps : ScriptableObject
    {
        public string Name;
        public float MinHeight;
        public float MaxHeight;
        public Obtainable ObtainablePrefab;
        public TileBase TileForTest;
        public float DistanceThreshold;
    }
}