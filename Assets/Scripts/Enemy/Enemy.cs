using FSM;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Enemy : NetworkBehaviour
{
    private Transform _hall;
    private Transform _target;
    private Vector3 _direction;

    [SerializeField] private List<TargetPriorities> _favouriteTargets;
    [SerializeField] private float _searchRadius;
    [SerializeField] private float _attackRadius;
    [SerializeField] private AttackType _attackType;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private MovementComponent _movementComponent;
    [SerializeField] private HealthComponent _healthComponent;
    [SerializeField] private EnemyPathFinder _pathFinder;
    [SerializeField] private AttackManager _attackManager;
    private IEnemyAttacker _enemyAttacker;

    private StateMachine _stateMachine = new StateMachine();
    private Animator _animator;

    public int damage = 10;
    public float cooldown = 1;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Awake()
    {

        _hall = GameObject.FindGameObjectWithTag("MainHall").transform;
        _movementComponent = GetComponent<MovementComponent>();
        _healthComponent = GetComponent<HealthComponent>();
        _pathFinder = GetComponent<EnemyPathFinder>();
        _attackManager = GetComponent<AttackManager>();

        _target = _hall;
        _pathFinder.target = _hall;
        _healthComponent.OnDeath += Cmd_Die;

        _attackManager.SetStrategy(GetComponent<IEnemyAttacker>());
        _attackManager.UpdateEnemy(this);
        _attackManager.UpdateTarget(_target.gameObject);

        _stateMachine.AddState("Move2Target", new State(
        onLogic: (state) =>
        {
            _movementComponent.MovementVector = _pathFinder.direction;
            _animator.SetBool("isMoving", true);
            if(_movementComponent.MovementVector.x > 0)
            {
                _animator.SetBool("isMovingRight", true);
            }
            else if (_movementComponent.MovementVector.x < 0)
            {
                _animator.SetBool("isMovingRight", false);
            }
            _movementComponent.Move();
        },
        onEnter: (state) => 
        { 
            Debug.Log($"Moving to target: {_target}");
        },
        onExit: (state) =>
        {
            _movementComponent.MovementVector = Vector3.zero;
        }));

        _stateMachine.AddState("UpdateTarget", new State(
        onEnter: (state) =>
        {
            Debug.Log($"Update target to: {_target}");
        }));

        _stateMachine.AddState("Attack", new State(
        onLogic: (state) =>
        {
            _attackManager.AttackTarget();
            //Debug.Log($"Target health: {_attackManager.GetTargetHealth()}");
        },
        onEnter: (state) =>
        {
            _animator.SetBool("isMoving", false);
            Debug.Log($"Attacked target: {_target}");
        }));

        _stateMachine.AddTransition(new Transition(
            "Move2Target",
            "UpdateTarget",
            TargetRefresh
        ));

        _stateMachine.AddTransition(new Transition(
            "UpdateTarget",
            "Move2Target",
            (transition) => _target != null
        ));

        _stateMachine.AddTransition(new Transition(
            "Move2Target",
            "Attack",
            TargetInAtackRange
        ));

        _stateMachine.AddTransition(new Transition(
            "Attack",
            "Move2Target",
            TargetOutAtackRange
        ));

        _stateMachine.AddTransition(new Transition(
            "Attack",
            "UpdateTarget",
            PlayerEntered
        ));

        _stateMachine.SetStartState("Move2Target");
        _stateMachine.Init();
    }
    private bool TargetRefresh(Transition<string> transition)
    {
        var previousTarget = _target;

        _target = FindTarget();
        _pathFinder.target = _target;

        var isCurrentTargetOpened = !_pathFinder.isTargetClosed;

        if (!_pathFinder.isTargetReachableThroughWall(_target))
        {
            GameObject wall = _pathFinder.Wall2Destroy;
            if (wall != null)
            {
                _target = wall.transform;
            }
        }

        if (_target != previousTarget) 
        {
            //_attackManager.UpdateTarget(_target.gameObject);
            _attackManager.SetTarget(_target);
            //_enemyAttacker.Target = _target;
            return true;
        };
        return false;
    }
    
    private bool TargetInAtackRange(Transition<string> transition)
    {
        return DistanceToTarget(_target) < _attackRadius;
    }

    private bool TargetOutAtackRange(Transition<string> transition)
    {
        return DistanceToTarget(_target) > _attackRadius && DistanceToTarget(_target) < _searchRadius;
    }

    private bool PlayerEntered(Transition<string> transition)
    {
        var playerTarget = FindTarget();
        if (_target.tag != "Wall" || playerTarget.tag != "Player")
        {
            return false;
        }

        _pathFinder.target = playerTarget;
        _pathFinder.Update();
        if (_pathFinder.isTargetClosed)
        {
            return false;
        }

        return true;

    }

    private Transform FindTarget()
    {
        foreach(TargetPriorities target in _favouriteTargets)
        {
            var targetTransformList = GameObject.FindGameObjectsWithTag(target.ToString())?
                .OrderBy(obj => DistanceToTarget(obj.transform));
            Transform targetTransform = targetTransformList.FirstOrDefault()?.transform;
            if (targetTransform!=null && DistanceToTarget(targetTransform) < _searchRadius)
            {
                return targetTransform;
            }
        }
        return _hall;
    }

    private void FixedUpdate()
    {
        _stateMachine.OnLogic();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }

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

    private IEnemyAttacker GetStratedyForEnemyType()
    {
        //string enemyTypeString = this.gameObject.GetType().ToString();
        //IEnemyAttacker attacker;
        switch (_attackType)
        {
            case AttackType.MeleeCharged:
                return new MeleeAttacker();
                //break;
            default:
                Debug.Log("There is no strategy: " + _attackType.ToString());
                return null;
        }
    }

    [Command(requiresAuthority = false)]
    private void Cmd_Die()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= Cmd_Die;
    }
}
