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

    [SerializeField] private List<ActiveSkill> _activeSkills;

    private Dictionary<int, KeyCode> _keyCodes;

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _keyCodes = new Dictionary<int, KeyCode>();
        for (int i = 0; i < _activeSkills.Count; i++)
        {
            _keyCodes.Add(i, (KeyCode)System.Enum.Parse(typeof(KeyCode), $"Alpha{i + 1}"));
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
        if (!isOwned) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("obtain");
            _animator.SetBool("IsObtaining", true);
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            _animator.SetBool("IsObtaining", false);
        }

        foreach (var code in _keyCodes)
        {
            if (Input.GetKeyDown(code.Value))
            {
                _activeSkills[code.Key].IsReady = true;
            }
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
