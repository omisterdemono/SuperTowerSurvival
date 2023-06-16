using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class MovementComponent : NetworkBehaviour
{
    public float Speed 
    {
        set
        {
            _speed = value;
        }
    }
    public Vector3 MovementVector
    {
        set
        {
            _movementVector = value;
        }

        get
        {
            return _movementVector;
        }
    }
    
    [SerializeField] private float _speed = 3;
    private Vector3 _movementVector;

    public void Move()
    {
        transform.position += _movementVector.normalized * _speed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        //Move();
    }
    void Start()
    {
        
    }
}
