using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private Animator _animator;

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("obtain");
            _animator.SetBool("IsObtaining", true);
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            _animator.SetBool("IsObtaining", false);
        }


    }

    public void TryObtain()
    {
        var instrument = GetComponentInChildren<Instrument>();
        if (!instrument)
        {
            return;
        }

        instrument.Obtain();

    }
}
