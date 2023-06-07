using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerSTS : NetworkManager
{
    //[SerializeField] List<GameObject> players;      fix this problem
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //GameObject player = Instantiate(players[numPlayers], Vector3.zero, Quaternion.identity); fix this problem
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }
}
