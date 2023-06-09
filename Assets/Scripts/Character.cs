using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
    }
    void FixedUpdate()
    {
        if (!isOwned) return;

        Vector3 moveVector = Vector3.zero;
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        moveVector.x = inputX;
        moveVector.y = inputY;

        _movement.MovementVector = moveVector;
        _movement.Move();
    }
}
