using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : NetworkBehaviour
{
    private void Start()
    {
        Invoke("DestroyExplosion", 3);
    }

    [Command(requiresAuthority = false)]
    private void DestroyExplosion()
    {
        NetworkServer.Destroy(gameObject);
    }
}
