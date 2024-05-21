using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Assets.Scripts.Weapons;
using System.Collections;
using Components;
using Infrastructure;
using Inventory;
using Inventory.UI;
using StructurePlacement;
using UnityEngine.EventSystems;
using System;
using System.Linq;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(NetworkTransform))]
public class Character : NetworkBehaviour
{
    private MovementComponent _movement;
    private HealthComponent _health;
    private Animator _animator;
    private HotBar _hotBar;
    private EffectComponent _effect;

    [Header("Fog Of War")]
    public FogOfWar fogOfWar;
    [Range(0, 20)]
    public float sightDistance;
    public float checkInterval;

    [SerializeField][SyncVar] private bool _isAlive = true;
    [SerializeField][SyncVar] private bool _isInvisible = false;
    private bool _canScrollTools = true;

    [SerializeField] private float _repairSpeedModifier = 1;
    [SerializeField] private float _buildSpeedModifier = 1;
    [SerializeField] private float _weaponDamageModifier = 1;

    //todo tools will be more generic in the future and these two list should be removed
    [SerializeField] private List<string> _toolIds = new();

    [SerializeField] private int _buildHammerSlotIndex = 1;
    
    [SyncVar(hook = nameof(HandleEquipedSlotChanged))]
    private int _equipedSlot = 0;

    [SyncVar(hook = nameof(HandleEquipableAnimation))]
    private bool _isPerforming;

    private List<GameObject> _tools = new();
    private List<ActiveSkill> _activeSkills;
    private List<IEquipable> _equipedTools = new();
    private PassiveSkill _passiveSkill;

    private StructurePlacer _structurePlacer;
    private PlayerInventory _playerInventory;
    private EquipSlot _equippedItemsSlot;
    private BuildHammer _buildHammer;

    private Vector2 _attackDirection;
    private Dictionary<int, KeyCode> _keyCodes;

    private Vector3 _mousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    private PowerUpStruct _currentLevel;

    public HealthComponent Health => _health;
    public StructurePlacer StructurePlacer => _structurePlacer;

    public bool IsAlive
    {
        get => _isAlive;
        set => _isAlive = value;
    }

    public float RepairSpeedModifier
    {
        get
        {
            if (_effect)
            {
                float tmpRepairSpeed = _repairSpeedModifier;
                foreach (var effect in _effect.Effects)
                {
                    if (effect.EffectType == EEffect.Repair)
                    {
                        tmpRepairSpeed = _effect.GetValue(tmpRepairSpeed, effect);
                    }
                }
                return tmpRepairSpeed;
            }

            return _repairSpeedModifier;
        }
        set => _repairSpeedModifier = value;
    }

    public float BuildSpeedModifier
    {
        get
        {
            if(_effect)
            {
                float tmpBuildSpeed = _buildSpeedModifier;
                foreach (var effect in _effect.Effects)
                {
                    if (effect.EffectType == EEffect.Build)
                    {
                        tmpBuildSpeed = _effect.GetValue(tmpBuildSpeed, effect);
                    }
                }
                return tmpBuildSpeed;
            }
            
            return _buildSpeedModifier;
        }
        set => _buildSpeedModifier = value;
    }

    public bool IsInvisible
    {
        get => _isInvisible;
        set => _isInvisible = value;
    }

    public int EquipedSlot { get => _equipedSlot; }
    public bool CanScrollTools { get => _canScrollTools; set => _canScrollTools = value; }

    public List<IEquipable> Equipables { get => _equipedTools; set => _equipedTools = value; }

    void Awake()
    {
        _movement = GetComponent<MovementComponent>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<HealthComponent>();
        _effect = GetComponent<EffectComponent>();

        _health.OnDeath += OnDeath;

        //change to something more generic
        _equippedItemsSlot = GetComponentInChildren<EquipSlot>();
        _playerInventory = GetComponent<PlayerInventory>();
        _buildHammer = GetComponentInChildren<BuildHammer>(true);
        _structurePlacer = GetComponent<StructurePlacer>();

        fogOfWar = FindFirstObjectByType<FogOfWar>();
    }

    private void Start()
    {
        InitTools();
        StartCoroutine(CheckFogOfWar(checkInterval));

        if (!isOwned) return;

        _activeSkills = new List<ActiveSkill>();
        _activeSkills.AddRange(GetComponents<ActiveSkill>());
        _passiveSkill = GetComponent<PassiveSkill>();
        _keyCodes = new Dictionary<int, KeyCode>();
        
        for (var i = 0; i < _activeSkills.Count; i++)
        {
            _keyCodes.Add(i, Config.GameConfig.ActiveSkillsKeyCodes[i]);
        }

        var gameInitializer = FindObjectOfType<GameInitializer>();
        gameInitializer.InitializeSkillHolder(_activeSkills);

        _hotBar = FindObjectOfType<HotBar>();

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().Target = transform;
    }

    private void InitTools()
    {
        var toolsAndStuff = new List<GameObject>();

        for (int i = 0; i < _equippedItemsSlot.gameObject.transform.childCount; i++)
        {
            toolsAndStuff.Add(_equippedItemsSlot.gameObject.transform.GetChild(i).gameObject);
        }

        var equipAbles = toolsAndStuff.Select(t => t.GetComponent<IEquipable>()).ToList();

        foreach (var toolId in _toolIds)
        {
            for (int i = 0; i < equipAbles.Count; i++)
            {
                if (equipAbles[i].Item.Id == toolId)
                {
                    _equipedTools.Add(equipAbles[i]);
                    _tools.Add(toolsAndStuff[i]);
                    break;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!isOwned || !_isAlive) return;

        HandleMove();
    }

    private IEnumerator CheckFogOfWar(float checkInterval)
    {
        while (true)
        {
            fogOfWar.MakeHole(transform.position, sightDistance);
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void HandleMove()
    {
        Vector3 moveVector = Vector3.zero;

        //float inputX = Input.GetAxisRaw("Horizontal");
        //float inputY = Input.GetAxisRaw("Vertical");

        float inputX = UserInput.instance.MoveInput.x;
        float inputY = UserInput.instance.MoveInput.y;

        _animator.SetInteger("x", (int)Math.Round(inputX));
        _animator.SetInteger("y", (int)Math.Round(inputY));

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
        HandleInventoryState();
    }

    private void HandleInventoryState()
    {
        if (Input.GetKeyDown(Config.GameConfig.OpenInventoryKeyCode))
        {
            _playerInventory.ChangeInventoryUIState();
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

    public void SelectInstrumentById(string id)
    {
        var index = _toolIds.IndexOf(id);
        CmdChangeTool(index);
    }

    private void HandleToolChanging()
    {
        if (!_canScrollTools)
        {
            return;
        }

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
            && !EventSystem.current.IsPointerOverGameObject())
        {
            Cmd_InteractOnServer(_mousePosition);
        }
        
        if (Input.GetKey(KeyCode.Mouse0)
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
        if (!_equipedTools[_equipedSlot].CanPerform)
        {
            return;
        }
        
        _isPerforming = true;
        _equipedTools[_equipedSlot].Interact();
        _equipedTools[_equipedSlot].MousePosition = mousePosition;
        StartCoroutine(FinishAnimation());
    }

    private IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(_equipedTools[_equipedSlot].CooldownSeconds - 0.5f);
        _isPerforming = false;
    }

    [Command(requiresAuthority = false)]
    private void Cmd_HoldOnServer()
    {
        if (!_equipedTools[_equipedSlot].CanPerform)
        {
            return;
        }
        
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
    
    public void PowerUp(PowerUpStruct powerUp)
    {
        PowerUpSkills(powerUp);
    }

    public void PowerUpSkills(PowerUpStruct powerUp)
    {
        (_activeSkills[0] as ISkill).PowerUpSkillPoint(powerUp.ActiveSkill1);
        (_activeSkills[1] as ISkill).PowerUpSkillPoint(powerUp.ActiveSkill2);
        _passiveSkill.PowerUp(powerUp.PassiveSkill);
        PowerUpHealth(powerUp.Health);
        PowerUpWeapon(powerUp.AttackDamage);
        PowerUpSpeed(powerUp.Speed);
        PowerUpBuild(powerUp.BuildSpeed);
    }

    public void PowerUpHealth(int points)
    {
    }

    public void PowerUpWeapon(int points)
    {
        
    }

    public void PowerUpSpeed(int points)
    {

    }

    public void PowerUpBuild(int points)
    {

    }
}