using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.EventSystems;
using Assets.Scripts.Weapons;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private HealthComponent _health;
    private Animator _animator;
    private ItemHolderScript _itemHolder;

    [SerializeField] private bool _isAlive = true;

    [SerializeField] private float _repairSpeedModifier = 1;
    [SerializeField] private float _buildSpeedModifier = 1;
    [SerializeField] private float _weaponDamageModifier = 1;

    private List<ActiveSkill> _activeSkills;

    private IWeapon _attacker;
    private EquipSlot _equippedItemSlot;
    private Action<Vector2> _performAttack;
    private Vector2 _attackDirection;

    private Dictionary<int, KeyCode> _keyCodes;

    private StructurePlacer _structurePlacer;

    [SerializeField] private List<GameObject> _toolSlots = new List<GameObject>();
    [SerializeField] private List<GameObject> _tools = new List<GameObject>();
    [SyncVar(hook = nameof(HandleEquipedSlotChanged))]
    private int _equipedSlot = 0;

    public bool IsAlive { get => _isAlive; set => _isAlive = value; }

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();

        //change to something more generic
        _attacker = GetComponentInChildren<IWeapon>();
        _equippedItemSlot = GetComponentInChildren<EquipSlot>();

        if (_attacker == null)
        {
            throw new NullReferenceException("Attacker script was not used on weapon");
        }

        _structurePlacer = GetComponent<StructurePlacer>();
    }

    private void Start()
    {
        _activeSkills = new List<ActiveSkill>();
        _activeSkills.AddRange(GetComponents<ActiveSkill>());
        _keyCodes = new Dictionary<int, KeyCode>();
        for (int i = 0; i < _activeSkills.Count; i++)
        {
            _keyCodes.Add(i, (KeyCode)System.Enum.Parse(typeof(KeyCode), $"Alpha{i + 1}"));
        }
        _itemHolder = GameObject.FindGameObjectWithTag("ItemHolder").GetComponent<ItemHolderScript>();

        _itemHolder.SetIcons(_tools);
    }

    void FixedUpdate()
    {
        if (!isOwned && _isAlive) return;

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

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            if (scrollInput > 0)
            {
                CmdChangeTool((_equipedSlot + 1) % 4);
            }
            else if (scrollInput < 0)
            {
                CmdChangeTool((_equipedSlot - 1 + 4) % 4);
            }

            _itemHolder.ChangeSlot(_equipedSlot);
        }

        //weapon handle and rotation
        HandleWeapon();

        //should be moved to build hammer
        HandleBuild();
    }
    private void HandleEquipedSlotChanged(int oldValue, int newValue)
    {
        _tools[oldValue].SetActive(false);
        _tools[newValue].SetActive(true);
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeTool(int equipedSlot)
    {
        _equipedSlot = equipedSlot;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_PressOnServer(Vector2 direction)
    {
        _attacker.Attack(direction);
    }

    [Command(requiresAuthority = false)]
    private void Cmd_HoldOnServer(Vector2 direction)
    {
        _attacker.Hold(direction);
    }

    [Command(requiresAuthority = false)]
    private void Cmd_KeyUpOnServer(Vector2 direction)
    {
        _attacker.KeyUp(direction);
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
            Cmd_PressOnServer(targetDirection.normalized);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Cmd_HoldOnServer(targetDirection.normalized);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Cmd_KeyUpOnServer(targetDirection.normalized);
        }
    }

    private void HandleEquippedItemRotation(IWeapon attacker, out Vector2 targetDirection)
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        targetDirection = worldPosition - (Vector2)transform.position;

        var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        _equippedItemSlot.Rotate(angle);
    }

    private void HandleBuild()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) 
            && !EventSystem.current.IsPointerOverGameObject()
            && _structurePlacer.GetTempStructureCanBePlaced()) // && 
        {
            _structurePlacer.PlaceStructure(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
