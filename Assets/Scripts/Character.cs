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

    [SerializeField] private List<ActiveSkill> _activeSkills;

    private IAttacker _attacker;
    private EquipedSlot _equippedItemSlot;
    private Action<Vector2> _performAttack;
    private Vector2 _attackDirection;

    private Dictionary<int, KeyCode> _keyCodes;

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();

        //change to something more generic
        _attacker = GetComponentInChildren<FireWeapon>();
        _equippedItemSlot = GetComponentInChildren<EquipedSlot>();

        if (_attacker == null)
        {
            throw new NullReferenceException("Attacker script was not used on weapon");
        }
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
        if (!isLocalPlayer) return;

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
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
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

        //weapon handle and rotation
        HandleWeapon();
    }

    [Command(requiresAuthority = false)]
    private void Cmd_AttackOnServer(Vector2 direction)
    {
        _attacker.Attack(direction, ref _performAttack);

        //_performAttack.Invoke(direction);
    }

    private void HandleWeapon()
    {
        if (_attacker == null)
        {
            return;
        }

        HandleEquippedItemRotation(_attacker, out Vector2 targetDirection);

        //rework, because some weapons have to be recharged
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cmd_AttackOnServer(targetDirection.normalized);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Cmd_AttackOnServer(targetDirection.normalized);
        }
    }

    private void HandleEquippedItemRotation(IAttacker attacker, out Vector2 targetDirection)
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        targetDirection = worldPosition - (Vector2)transform.position;

        var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        _equippedItemSlot.Rotate(angle);
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
