using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour 
{
    [SerializeField] private float speed = 3f;

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        Vector3 moveDirection = Vector3.zero;

        if(Input.GetKey(KeyCode.W)) moveDirection.y = 1f;
        if(Input.GetKey(KeyCode.S)) moveDirection.y = -1f;
        if(Input.GetKey(KeyCode.D)) moveDirection.x = 1f;
        if(Input.GetKey(KeyCode.A)) moveDirection.x = -1f;

        transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }
}
