using Mirror;
using UnityEngine;

public class NetworkManagerSTS : NetworkManager
{
    [Header("Character setup")]
    [SerializeField] private int _choosenCharacter = 0;
    [SerializeField] private GameObject[] _characters;
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
