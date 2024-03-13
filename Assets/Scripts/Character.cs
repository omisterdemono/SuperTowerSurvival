using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Assets.Scripts.Weapons;
using System.Collections;
using Infrastructure;
using Inventory;
using Inventory.UI;
using StructurePlacement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private HealthComponent _health;
    private Animator _animator;
    private HotBar _hotBar;

    [SerializeField] [SyncVar] private bool _isAlive = true;
    [SerializeField] [SyncVar] private bool _isInvisible = false;

    [SerializeField] private float _repairSpeedModifier = 1;
    [SerializeField] private float _buildSpeedModifier = 1;
    [SerializeField] private float _weaponDamageModifier = 1;

    //todo tools will be more generic in the future and these two list should be removed
    [SerializeField] private List<GameObject> _tools = new();
    [SerializeField] private List<string> _toolIds = new();

    [SerializeField] private int _buildHammerSlotIndex = 1;

    [SyncVar(hook = nameof(HandleEquipedSlotChanged))]
    private int _equipedSlot = 0;

    [SyncVar(hook = nameof(HandleEquipableAnimation))]
    private bool _isPerforming;

    private List<ActiveSkill> _activeSkills;
    private List<IEquipable> _equipedTools = new();

    private StructurePlacer _structurePlacer;
    private PlayerInventory _playerInventory;
    private EquipSlot _equippedItemsSlot;
    private BuildHammer _buildHammer;

    private Vector2 _attackDirection;
    private Dictionary<int, KeyCode> _keyCodes;

    private Vector3 _mousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    public HealthComponent Health => _health;
    public StructurePlacer StructurePlacer => _structurePlacer;

    public bool IsAlive
    {
        get => _isAlive;
        set => _isAlive = value;
    }

    public float RepairSpeedModifier
    {
        get => _repairSpeedModifier;
        set => _repairSpeedModifier = value;
    }

    public float BuildSpeedModifier
    {
        get => _buildSpeedModifier;
        set => _buildSpeedModifier = value;
    }

    public bool IsInvisible
    {
        get => _isInvisible;
        set => _isInvisible = value;
    }


    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<HealthComponent>();

        _health.OnDeath += OnDeath;

        //change to something more generic
        _equippedItemsSlot = GetComponentInChildren<EquipSlot>();
        _playerInventory = GetComponent<PlayerInventory>();
        _buildHammer = GetComponentInChildren<BuildHammer>(true);
        _structurePlacer = GetComponent<StructurePlacer>();

        //change to something more generic
        foreach (var tool in _tools)
        {
            _equipedTools.Add(tool.GetComponent<IEquipable>());
        }
    }

    private void Start()
    {
        if (!isOwned) return;

        _activeSkills = new List<ActiveSkill>();
        _keyCodes = new Dictionary<int, KeyCode>();

        _activeSkills.AddRange(GetComponents<ActiveSkill>());
        for (var i = 0; i < _activeSkills.Count; i++)
        {
            _keyCodes.Add(i, Config.GameConfig.ActiveSkillsKeyCodes[i]);
        }

        var gameInitializer = FindObjectOfType<GameInitializer>();
        gameInitializer.InitializeSkillHolder(_activeSkills);

        _hotBar = FindObjectOfType<HotBar>();

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().Target = transform;
    }

    void FixedUpdate()
    {
        if (!isOwned || !_isAlive) return;

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
        HandleInventoryState();
    }

    private void HandleInventoryState()
    {
        if (Input.GetKeyDown(Config.GameConfig.OpenInventoryKeyCode))
        {
            _playerInventory.ChangeInventoryUIState();
            Cursor.visible = !Cursor.visible;
        }
    }

    private void HandleBuildHammerState()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2) && _equipedSlot == _buildHammerSlotIndex)
        {
            if (_buildHammer.CurrentState == BuildHammerState.Building)
            {
                _structurePlacer.CancelPlacement();
            }

            ChangeBuildHammerStateOnServer();

            if (isLocalPlayer)
            {
                //_buildHammer.ChangeMode();
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void ChangeBuildHammerStateOnServer()
    {
        _buildHammer.ChangeMode();
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

    public void SelectInstrumentById(string id)
    {
        var index = _toolIds.IndexOf(id);
        CmdChangeTool(index);
    }

    private void HandleToolChanging()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");
        var itemWasUsable = true;

        for (var index = 0; index < Config.GameConfig.HotbarKeyCodes.Count; index++)
        {
            var code = Config.GameConfig.HotbarKeyCodes[index];
            if (Input.GetKeyDown(code))
            {
                itemWasUsable = _hotBar.ActivateCell(index, this);
            }
        }

        switch (scrollInput)
        {
            case > 0:
                var slotNumberUp = (_hotBar.SelectedCell + 1) % Config.GameConfig.HotbarCellsCount;
                itemWasUsable = _hotBar.ActivateCell(slotNumberUp, this);
                break;
            case < 0:
                var slotNumberDown = (_hotBar.SelectedCell - 1 + Config.GameConfig.HotbarCellsCount)
                                     % Config.GameConfig.HotbarCellsCount;
                itemWasUsable = _hotBar.ActivateCell(slotNumberDown, this);
                break;
        }
        
        if (!itemWasUsable)
        {
            CmdChangeTool(-1);
        }

        if (_equipedSlot >= _tools.Count || _equipedSlot == -1)
        {
            return;
        }

        _equippedItemsSlot.ChangeRotatingChild(_equipedSlot);
    }

    private void HandleEquippedTool()
    {
        if (_equipedTools.Count == 0 || _equipedSlot < 0 || _equipedSlot >= _tools.Count)
        {
            return;
        }

        HandleEquippedItemRotation();

        if (Input.GetKeyDown(KeyCode.Mouse0)
            && _equipedTools[_equipedSlot].CanPerform
            && !EventSystem.current.IsPointerOverGameObject())
        {
            Cmd_InteractOnServer(_mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0)
            && _equipedTools[_equipedSlot].CanPerform
            && !EventSystem.current.IsPointerOverGameObject())
        {
            Cmd_HoldOnServer();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Cmd_FinishHoldOnServer();
        }
    }

    private void OnDeath()
    {
        IsAlive = false;
        _animator.SetBool("IsAlive", false);
    }

    private void HandleEquipedSlotChanged(int oldValue, int newValue)
    {
        if ((oldValue != -1 && oldValue < _tools.Count) //so we do not leave boundaries of tools array available 
            || (newValue == -1 && oldValue != -1)) //so we hide unused tool and have no troubles
        {
            _tools[oldValue].SetActive(false);
        }

        if (newValue != -1 && newValue < _tools.Count)
        {
            _tools[newValue].SetActive(true);
        }
    }

    private void HandleEquipableAnimation(bool oldValue, bool newValue)
    {
        _equipedTools[_equipedSlot].ChangeAnimationState();
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeTool(int equipedSlot)
    {
        _equipedSlot = equipedSlot;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_InteractOnServer(Vector3 mousePosition)
    {
        _isPerforming = true;
        _equipedTools[_equipedSlot].Interact();
        _equipedTools[_equipedSlot].MousePosition = mousePosition;

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