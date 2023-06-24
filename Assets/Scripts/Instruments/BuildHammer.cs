using Assets.Scripts.Weapons;
using System;
using UnityEngine;

enum BuildHammerStates
{
    Repairing,
    Building
}

public class BuildHammer : MonoBehaviour, IInstrument, IEquipable
{
    [SerializeField] private InstrumentAttributes _instrumentAttributes;
    [SerializeField] private float _cooldownSeconds;

    public InstrumentType InstrumentType { get; set; }

    public float Strength { get; set; }
    public float Durability { get; set; }
    public bool NeedFlip { get; set; }
    public bool NeedRotation { get; set; }

    private Animator _animator;
    private StructurePlacer _structurePlacer;
    private CooldownComponent _cooldownComponent;
    private HealthComponent _currentStructureToRepair;
    private BuildHammerStates _currentState = BuildHammerStates.Building;
    private bool _isObtaining = false;

    private Vector3 _mousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void Awake()
    {
        Strength = _instrumentAttributes.Strength;
        Durability = _instrumentAttributes.Durability;
        InstrumentType = _instrumentAttributes.InstrumentType;

        _animator = GetComponent<Animator>();
        _structurePlacer = GetComponentInParent<StructurePlacer>();
        _cooldownComponent = new CooldownComponent() { CooldownSeconds = _cooldownSeconds };
    }

    private void Update()
    {
        _cooldownComponent.HandleCooldown();
    }

    public void Interact()
    {
        if (!_cooldownComponent.CanHit)
        {
            return;
        }
        _cooldownComponent.ResetCooldown();

        ChangeObtainingState();
    }

    private void ChangeObtainingState()
    {
        _isObtaining = !_isObtaining;
        _animator.SetBool("isObtaining", _isObtaining);
    }

    public void Hold()
    {
        Interact();
    }

    public void FinishHold()
    {
        return;
    }

    public void Obtain()
    {
        switch (_currentState)
        {
            case BuildHammerStates.Repairing:
                if (!_currentStructureToRepair)
                {
                    return;
                }
                Repair();
                break;
            case BuildHammerStates.Building:
                Place();
                break;
            default:
                break;
        }

        ChangeObtainingState();
    }

    private void Repair()
    {
        _currentStructureToRepair.Heal(Strength);
        Durability -= 1.0f;
    }

    private void Place()
    {
        if (!_structurePlacer.StructureCanBePlaced) 
        {
            return;
        }

        _structurePlacer.PlaceStructure(_mousePosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //saving for repair
        if (collision.TryGetComponent<Structure>(out var component))
        {
            _currentStructureToRepair = component.GetComponent<HealthComponent>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //removing from repair
        if (collision.TryGetComponent<Structure>(out var component))
        {
            _currentStructureToRepair = null;
        }
    }
}
