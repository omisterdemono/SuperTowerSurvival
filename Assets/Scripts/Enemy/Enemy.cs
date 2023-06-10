using FSM;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Enemy : NetworkBehaviour
{
    private GameObject _mainHall;
    private GameObject _target;

    [SerializeField] private float _searchRadius;
    [SerializeField] private float _attackRadius;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private MovementComponent _movementComponent;

    private StateMachine _stateMachine = new StateMachine();

    private void Awake() //override
    {
        _mainHall = GameObject.FindGameObjectWithTag("MainHall");

        _movementComponent = GetComponent<MovementComponent>();

        //states
        _stateMachine.AddState("Move2Hall", new State(
        onLogic: (state) =>
        {
            _movementComponent.MovementVector = (_mainHall.transform.position - transform.position).normalized;
            _movementComponent.Move();
        },
        onEnter: (state) =>
        {
            Debug.Log("Moving to Hall");
        }));

        _stateMachine.AddState("Move2Target", new State(
        onLogic: (state) =>
        {
            _movementComponent.MovementVector = (_playerTransform.position - transform.position).normalized;
            _movementComponent.Move();
        },
        onEnter: (state) => 
        { 
            Debug.Log("Moving to target");
        },
        onExit: (state) =>
        {
            _movementComponent.MovementVector = Vector3.zero;
        }));

        _stateMachine.AddState("Attack", new State(
        onEnter: (state) =>
        {
            Debug.Log("Attacking target!");
        }));

        //transitions
        //Move2Hall - Move2Target 
        _stateMachine.AddTransition(new Transition(
            "Move2Hall",
            "Move2Target",
            targetInSightRange
        ));

        _stateMachine.AddTransitionFromAny(new Transition(
            "",
            "Move2Hall",
            targetOutSightRange
        ));

        //Move2Target - attack
        _stateMachine.AddTransition(new Transition(
            "Move2Target",
            "Attack",
            targetInAtackRange
        ));

        _stateMachine.AddTransition(new Transition(
            "Attack",
            "Move2Target",
            targetOutAtackRange
        ));

        //init
        _stateMachine.SetStartState("Move2Hall");
        _stateMachine.Init();
    }

    private bool targetInSightRange(Transition<string> transition)
    {
        return _playerTransform != null && DistanceToPlayer() < _searchRadius;
    }
    private bool targetOutSightRange(Transition<string> transition)
    {
        return DistanceToPlayer() > _searchRadius;
    }
    
    private bool targetInAtackRange(Transition<string> transition)
    {
        return DistanceToPlayer() < _attackRadius;
    }

    private bool targetOutAtackRange(Transition<string> transition)
    {
        return DistanceToPlayer() > _attackRadius && DistanceToPlayer() < _searchRadius;
    }

    private void Update()
    {
        _stateMachine.OnLogic();
    }

    private void OnDrawGizmosSelected()
    {
        //Search
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        //Attack
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }

    private float DistanceToPlayer()
    {
        var distance = Vector3.Distance(transform.position, _playerTransform.position);
        return distance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();

        if (character)
        {
            _playerTransform = character.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var character = collision.GetComponent<Character>();

        if (character.transform == _playerTransform)
        {
            _playerTransform = null;
        }
    }
}
