using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int _minPlayers = 1;
    [Scene] [SerializeField] private string _menuScene = string.Empty;

    [Header("Room")] [SerializeField] private NetworkRoomPlayerLobby _roomPlayerLobbyPrefab = null;

    [Header("Game")] [SerializeField] private NetworkGamePlayer _gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private string _newSceneName = "Scene_Map_02";

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    private List<int> _chosenCharacters = new List<int>();

    public override void OnStartClient()
    {
        var spawnableObjects = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnableObjects)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SceneManager.LoadScene(offlineScene);
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != _menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().path == _menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;
            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(_roomPlayerLobbyPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < _minPlayers) return false;

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) return false;
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == _menuScene)
        {
            if (!IsReadyToStart()) return;
            _chosenCharacters.AddRange(RoomPlayers.Select(x => x.CurrentCharacter));
            ServerChangeScene(_newSceneName);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == _menuScene && newSceneName.StartsWith("Scene_Map"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayerIstance = Instantiate(_gamePlayerPrefab);

                gamePlayerIstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerIstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("Scene_Map"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            playerSpawnSystemInstance.GetComponent<PlayerSpawnSystem>().ChosenCharacters.AddRange(_chosenCharacters);
            NetworkServer.Spawn(playerSpawnSystemInstance);
            Debug.Log("spawned player spawn system");
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
        Debug.Log("on server ready");
    }
}