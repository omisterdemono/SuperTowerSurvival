using Mirror;
using UnityEngine;

namespace Infrastructure
{
    public class NetworkGameInitializer : NetworkBehaviour
    {
        [Header("Map generation")] 
        [SerializeField] private MapGenerator _landGenerator;
        [SerializeField] private MapGenerator _resourceGenerator;
        
        [Header("UI")]
        [SerializeField] private Canvas _waitCanvas;

        private void Awake()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            
            GenerateMap();
            SpawnPlayers();
        }
        
        private void SpawnPlayers()
        {
            var playerSpawner = FindObjectOfType<PlayerSpawnSystem>();
            foreach (var (key, value) in NetworkServer.connections)
            {
                playerSpawner.SpawnPlayer(value);
                Debug.Log($"[Game Initializer] Player {key} spawned");
            }
        }

        public void GenerateMap()
        {
            System.Random random = new();
            var seed = random.Next(0, 10000);

            GenerateMaps(seed);
            Debug.Log("[Game Initializer] Generated maps with seed " + seed);

            FinishWaiting();
        }

        [ClientRpc]
        public void FinishWaiting()
        {
            HideWaitingCanvas();
        }
        
        public void GenerateMaps(int seed)
        {
            _landGenerator.GenerateMap(seed);
            _resourceGenerator.GenerateMap(seed);
        }

        public void HideWaitingCanvas()
        {
            _waitCanvas.gameObject.SetActive(false);
        }
    }
}