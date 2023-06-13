using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private HealthComponent _health;
    private Animator _animator;

    [SerializeField] private float _repairSpeedModifier = 1;
    [SerializeField] private float _buildSpeedModifier = 1;
    [SerializeField] private float _weaponDamageModifier = 1;

    [SerializeField] private List<ActiveSkill> _activeSkill;

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();
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

        //weapon handle and rotation
        HandleWeapon();
    }

    private void HandleWeapon()
    {
        var attacker = GetComponentInChildren<IAttacker>();
        HandleWeaponRotation(attacker);

        //rework, because some weapons have to be recharged
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            attacker.Attack();
        }

    }

    private void HandleWeaponRotation(IAttacker attacker)
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 targetDirection = worldPosition - (Vector2)transform.position;

        var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        attacker.Rotate(angle);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _activeSkill[0].IsReady = true;
        }
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
