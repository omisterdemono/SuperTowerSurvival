using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerSTS : NetworkManager
{
    [SerializeField] private GameObject[] _characters;
    public int _choosenCharacter;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(_characters[_choosenCharacter], Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, _characters[_choosenCharacter].GetComponent<NetworkIdentity>().assetId);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }
}
