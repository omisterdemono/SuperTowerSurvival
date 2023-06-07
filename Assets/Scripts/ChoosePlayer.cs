using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChoosePlayer : NetworkBehaviour
{
    [SerializeField] List<GameObject> players;

    public void Select(int index)
    {
        Choose(index);
    }
    [Command(requiresAuthority = false)]
    public void Choose(int index, NetworkConnectionToClient sender = null)
    {
        GameObject player = Instantiate(players[index], Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(player, sender);
    }
}
