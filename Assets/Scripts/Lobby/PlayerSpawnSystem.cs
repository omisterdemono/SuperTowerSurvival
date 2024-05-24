using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private List<GameObject> playerPrefabs = new List<GameObject>();

    [SyncVar]
    public List<int> ChosenCharacters = new List<int>();

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        Debug.Log("add spawnpoint");
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

    [ServerCallback]

    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player{nextIndex}");
            return;
        }

        GameObject playerIstance = Instantiate(playerPrefabs[ChosenCharacters[nextIndex]], spawnPoints[nextIndex].position, Quaternion.identity);
        NetworkServer.Spawn(playerIstance, conn);

        if (nextIndex == ChosenCharacters.Count - 1)
        {
            //last player spawned
            StartWaiting();
            System.Random random = new();
            var gameInit = FindObjectOfType<GameInitializer>();
            var seed = random.Next(0, 10000);
            gameInit.GenerateMaps(seed);
            FinishWaiting();
            
            Debug.Log("generated maps with seed " + seed);
        }
        
        nextIndex++;
    }
    
    [ClientRpc]
    public void StartWaiting()
    {
        FindObjectOfType<GameInitializer>().ShowWaitingCanvas();
    }

    [ClientRpc]
    public void FinishWaiting()
    {
        FindObjectOfType<GameInitializer>().HideWaitingCanvas();
    }
}
