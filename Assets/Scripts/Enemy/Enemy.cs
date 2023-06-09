using FSM;
using Mirror;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] private float _searchRadius;
    [SerializeField] private float _attackRadius;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private MovementComponent _movementComponent;

    private StateMachine _stateMachine = new StateMachine();

    private void Awake() //override
    {
        _movementComponent = GetComponent<MovementComponent>();

        //states
        _stateMachine.AddState("Idle", new State());

        _stateMachine.AddState("FollowPlayer", new State(
        onLogic: (state) =>
        {
            _movementComponent.MovementVector = (_playerTransform.position - transform.position).normalized;
            _movementComponent.Move();
        },
        onExit: (state) =>
        {
            _movementComponent.MovementVector = Vector3.zero;
        }));

        _stateMachine.AddState("Attack", new State(
        onLogic: (state) =>
        {
            Debug.Log("Attacking!!!");
        }));

        //transitions
        //idle - follow 
        _stateMachine.AddTransition(new Transition(
            "Idle",
            "FollowPlayer",
            (transition) => _playerTransform != null && DistanceToPlayer() < _searchRadius
        ));

        _stateMachine.AddTransitionFromAny(new Transition(
            "",
            "Idle",
            (transition) => DistanceToPlayer() > _searchRadius
        ));

        //follow - attack
        _stateMachine.AddTransition(new Transition(
            "FollowPlayer",
            "Attack",
            (transition) => DistanceToPlayer() < _attackRadius
        ));

        _stateMachine.AddTransition(new Transition(
            "Attack",
            "FollowPlayer",
            (transition) => DistanceToPlayer() > _attackRadius
        ));

        //init
        _stateMachine.SetStartState("Idle");
        _stateMachine.Init();
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
