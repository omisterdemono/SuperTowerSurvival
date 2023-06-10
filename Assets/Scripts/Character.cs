using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private Animator _animator;
    
    [SerializeField] private Animation _obtainAnimation;

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

        if (Input.GetKeyDown(KeyCode.F))
        {
            _obtainAnimation.Play("Collect");
            //_animator.SetTrigger("Obtain");
        }
    }

    public void TryObtain()
    {
        var instrument = GetComponentInChildren<Pickaxe>();
        if (!instrument)
        {
            return;
        }

        instrument.Obtain();
    }
}
