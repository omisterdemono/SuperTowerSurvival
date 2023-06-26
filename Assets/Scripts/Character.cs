using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Assets.Scripts.Weapons;
using System.Collections;

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
    [SerializeField] private List<GameObject> _tools = new();

    [SerializeField] private int _buildHammerSlotIndex = 1;

    [SyncVar(hook = nameof(HandleEquipedSlotChanged))] private int _equipedSlot = 0;
    [SyncVar(hook = nameof(HandleEquipableAnimation))] private bool _isPerforming;

    private List<ActiveSkill> _activeSkills;
    private List<IEquipable> _equipedTools = new();

    private StructurePlacer _structurePlacer;
    private EquipSlot _equippedItemsSlot;

    private BuildHammer _buildHammer;

    private Vector2 _attackDirection;
    private Dictionary<int, KeyCode> _keyCodes;


    public bool IsAlive { get => _isAlive; set => _isAlive = value; }

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();

        _equippedItemsSlot = GetComponentInChildren<EquipSlot>();
        _buildHammer = GetComponentInChildren<BuildHammer>(true);

        //change to something more generic
        foreach (var tool in _tools)
        {
            _equipedTools.Add(tool.GetComponent<IEquipable>());
        }
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

        HandleMove();
    }

    private void HandleMove()
    {
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

        HandleActiveSkills();
        HandleToolChanging();
        HandleEquippedTool();
        HandleBuildHammerState();
    }

    private void HandleBuildHammerState()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2) && _equipedSlot == _buildHammerSlotIndex)
        {
            _buildHammer.ChangeMode();
        }
    }

    private void HandleActiveSkills()
    {
        foreach (var code in _keyCodes)
        {
            if (Input.GetKeyDown(code.Value))
            {
                _activeSkills[code.Key].IsReady = true;
            }
        }
    }

    private void HandleToolChanging()
    {
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
            _equippedItemsSlot.ChangeRotatingChild(_equipedSlot);
        }
    }

    private void HandleEquippedTool()
    {
        if (_equipedTools.Count == 0)
        {
            return;
        }

        HandleEquippedItemRotation();

        if (Input.GetKeyDown(KeyCode.Mouse0) && _equipedTools[_equipedSlot].CanPerform)
        {
            Cmd_InteractOnServer();
        }

        if (Input.GetKey(KeyCode.Mouse0) && _equipedTools[_equipedSlot].CanPerform)
        {
            Cmd_HoldOnServer();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Cmd_FinishHoldOnServer();
        }
    }

    private void HandleEquipableAnimation(bool oldValue, bool newValue)
    {
        _equipedTools[_equipedSlot].ChangeAnimationState();
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
    private void Cmd_InteractOnServer()
    {
        _isPerforming = true;
        _equipedTools[_equipedSlot].Interact();

        StartCoroutine(FinishAnimation());
    }

    private IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(_equipedTools[_equipedSlot].CooldownSeconds);
        _isPerforming = false;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_HoldOnServer()
    {
        _isPerforming = true;
        _equipedTools[_equipedSlot].Hold();

        StartCoroutine(FinishAnimation());
    }

    [Command(requiresAuthority = false)]
    private void Cmd_FinishHoldOnServer()
    {
        _equipedTools[_equipedSlot].FinishHold();
    }

    private void HandleEquippedItemRotation()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 targetDirection = worldPosition - (Vector2)transform.position;

        var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        _equippedItemsSlot.Rotate(angle);
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
