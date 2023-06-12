using FSM;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Enemy : NetworkBehaviour
{
    private GameObject _mainHall;
    private Transform _target;

    [SerializeField] private List<TargetPriorities> _favouriteTargets;
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
            _movementComponent.MovementVector = (_target.position - transform.position).normalized;
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

        _stateMachine.AddState("UpdateTarget", new State(
        onEnter: (state) =>
        {
            //FindTarget();
            Debug.Log("Update the target");
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

        //Move2Target - UpdateTarget
        _stateMachine.AddTransitionFromAny(new Transition(
            "",
            "UpdateTarget",
            targetRefresh
        ));

        //UpdateTarget - Move2Target
        _stateMachine.AddTransition(new Transition(
            "UpdateTarget",
            "Move2Target",
            (transition) => _target != null
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
    private bool targetRefresh(Transition<string> transition)
    {
        var currentTarget = _target;
        var newTarget = FindTarget();
        return newTarget != currentTarget;
    }


    private bool targetInSightRange(Transition<string> transition)
    {
        FindTarget();
        return _target != null;
    }
    private bool targetOutSightRange(Transition<string> transition)
    {
        bool distance = DistanceToTarget(_target) > _searchRadius;
        if (distance)
        {
            _target = null;
            return true;
        }
        return false;
    }
    
    private bool targetInAtackRange(Transition<string> transition)
    {
        return DistanceToTarget(_target) < _attackRadius;
    }

    private bool targetOutAtackRange(Transition<string> transition)
    {
        return DistanceToTarget(_target) > _attackRadius && DistanceToTarget(_target) < _searchRadius;
    }

    private Transform FindTarget()
    {
        foreach(TargetPriorities target in _favouriteTargets)
        {
            var targetTransformList = GameObject.FindGameObjectsWithTag(target.ToString())?
                .OrderBy(obj => DistanceToTarget(obj.transform)); ;
            Transform targetTransform = targetTransformList.FirstOrDefault()?.transform;
            if (targetTransform!=null && DistanceToTarget(targetTransform) < _searchRadius)
            {
                _target = targetTransform;
                return targetTransform;
            }
            //_target = null;
        }
        return null;
    }

    private void Update()
    {
        _stateMachine.OnLogic();
    }

    //private void OnDrawGizmosSelected()
    //{
    //    //Search
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, _searchRadius);

    //    //Attack
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _attackRadius);
    //}

    private float DistanceToTarget(Transform target)
    {
        var distance = Vector3.Distance(transform.position, target.position);
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

        if (character && character.transform == _playerTransform)
        {
            _playerTransform = null;
        }
    }
}
