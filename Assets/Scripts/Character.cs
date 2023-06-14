using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public delegate void PerformAttack(Vector2 direction);

public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private HealthComponent _health;
    private Animator _animator;

    [SerializeField] private float _repairSpeedModifier = 1;
    [SerializeField] private float _buildSpeedModifier = 1;
    [SerializeField] private float _weaponDamageModifier = 1;

    [SerializeField] private List<ActiveSkill> _activeSkill;

    private IAttacker _attacker;
    private Action<Vector2> _performAttack;
    private Vector2 _attackDirection;

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();

        //change to something more generic
        _attacker = GetComponentInChildren<FireWeapon>();

        if (_attacker == null)
        {
            throw new NullReferenceException("Attacker script was not used on weapon");
        }
    }

    void FixedUpdate()
    {
        if (!isOwned) return;

        Vector3 moveVector = Vector3.zero;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        _animator.SetInteger("x", (int)inputX);
        _animator.SetInteger("y", (int)inputY);

        moveVector.x = inputX;
        moveVector.y = inputY;

        _movement.MovementVector = moveVector;
        _movement.Move();
    }

    private void Update()
    {
        if (!isOwned) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _activeSkill[0].IsReady = true;
        }

        //weapon handle and rotation
        HandleWeapon();
    }

    private void HandleWeapon()
    {
        if (_attacker == null)
        {
            return;
        }

        HandleWeaponRotation(_attacker, out Vector2 targetDirection);

        //rework, because some weapons have to be recharged
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _attacker.Attack(targetDirection.normalized, ref _performAttack);
            _attackDirection= targetDirection.normalized;

            Cmd_AttackOnServer();
        }

    }

    private void HandleWeaponRotation(IAttacker attacker, out Vector2 targetDirection)
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        targetDirection = worldPosition - (Vector2)transform.position;

        var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        attacker.Rotate(angle);
    }

    [ClientRpc]
    private void Rpc_Attack()
    {
        Cmd_AttackOnServer();
    }

    [Command(requiresAuthority = false)]
    private void Cmd_AttackOnServer()
    {
        _performAttack.Invoke(_attackDirection);
    }

    public void PowerUpHealth()
    {
        //todo power up health
    }

    public void PowerUpSkill()
    {

    }

    public void PowerUpWeapon()
    {
        //todo power up weapon
    }

}
