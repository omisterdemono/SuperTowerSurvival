using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private List<GameObject> playerPrefabs;

    [SyncVar]
    public List<int> ChosenCharacters = new();

    private static List<Transform> spawnPoints;

    private int nextIndex = 0;

    private void Awake()
    {
        SavePlayersChoice();
        FindSpawnPoints();
        Debug.Log("[Game init] PlayerSpawnSystem Start");
    }

    private void FindSpawnPoints()
    {
        spawnPoints = FindObjectsOfType<PlayerSpawnPoint>().Select(sp => sp.transform).ToList();
    }

    private void SavePlayersChoice()
    {
        ChosenCharacters.AddRange(FindObjectOfType<NetworkManagerLobby>().ChosenCharacters);
    }

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        Debug.Log("add spawnpoint");
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        var server = isServer;
        
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player{nextIndex}");
            return;
        }

        GameObject playerIstance = Instantiate(playerPrefabs[ChosenCharacters[nextIndex]], spawnPoints[nextIndex].position, Quaternion.identity);
        NetworkServer.Spawn(playerIstance, conn);
        nextIndex++;
    }
}
