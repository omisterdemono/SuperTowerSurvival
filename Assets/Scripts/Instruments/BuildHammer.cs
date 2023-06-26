using Assets.Scripts.Weapons;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

enum BuildHammerState
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
    public bool NeedFlip { get; set; } = true;
    public bool NeedRotation { get; set; } = true;
    public bool CanPerform => _cooldownComponent.CanPerform;
    public float CooldownSeconds => _cooldownSeconds;

    private Animator _animator;
    private StructurePlacer _structurePlacer;
    private CooldownComponent _cooldownComponent;
    private HealthComponent _currentStructureToRepair;
    private BuildHammerState _currentState = BuildHammerState.Building;
    private bool _isObtaining;

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
        if (!_cooldownComponent.CanPerform)
        {
            return;
        }
        _cooldownComponent.ResetCooldown();

        StartCoroutine(DelayedObtain());
    }

    public void ChangeAnimationState()
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

    private IEnumerator DelayedObtain()
    {
        yield return new WaitForSeconds(_cooldownSeconds);
        Obtain();
    }

    public void Obtain()
    {
        switch (_currentState)
        {
            case BuildHammerState.Repairing:
                Repair();
                break;
            case BuildHammerState.Building:
                Place();
                break;
            default:
                break;
        }
    }

    private void Repair()
    {
        if (!_currentStructureToRepair)
        {
            return;
        }

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

    public void ChangeMode()
    {
        if (_currentState == BuildHammerState.Building)
        {
            _structurePlacer.CancelPlacement();
        }

        var highestState = Enum.GetValues(typeof(BuildHammerState)).Cast<BuildHammerState>().Max();

        if (_currentState == highestState)
        {
            _currentState = 0;
            return;
        }

        _currentState++;
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
