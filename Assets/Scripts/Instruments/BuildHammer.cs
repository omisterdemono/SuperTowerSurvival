using Assets.Scripts.Weapons;
using System.Collections;
using Components;
using Inventory.Models;
using StructurePlacement;
using Structures;
using UnityEngine;
using UnityEngine.Serialization;

public enum BuildHammerState
{
    Repairing,
    Building
}

public class BuildHammer : MonoBehaviour, IInstrument, IEquipable
{
    [FormerlySerializedAs("_instrumentAttributes")] [SerializeField] private InstrumentItemSO _instrumentItemSo;
    [SerializeField] private float _cooldownSeconds;

    public InstrumentType InstrumentType { get; set; }

    public float Strength { get; set; }
    public float Durability { get; set; }
    public bool NeedFlip { get; set; } = true;
    public bool NeedRotation { get; set; } = true;
    public bool CanPerform => _cooldownComponent.CanPerform;
    public float CooldownSeconds => _cooldownSeconds;
    public Vector3 MousePosition { get => mousePosition; set => mousePosition = value; }

    private Vector3 mousePosition;

    private Animator _animator;
    private StructurePlacer _structurePlacer;
    private CooldownComponent _cooldownComponent;
    private HealthComponent _currentStructureToRepair;
    public BuildHammerState CurrentState = BuildHammerState.Building;
    private bool _isObtaining;

    private void Awake()
    {
        Strength = _instrumentItemSo.Strength;
        Durability = _instrumentItemSo.Durability;
        InstrumentType = _instrumentItemSo.InstrumentType;

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
        HandleChangeMode();
        
        switch (CurrentState)
        {
            case BuildHammerState.Repairing:
                Repair();
                break;
            case BuildHammerState.Building:
                Place();
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

        _structurePlacer.PlaceStructure(MousePosition);
    }

    public void HandleChangeMode()
    {
        CurrentState = _structurePlacer.CurrentStructureId != string.Empty ? BuildHammerState.Building : BuildHammerState.Repairing;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is not BoxCollider2D)
        {
            return;
        }

        //saving for repair
        if (collision.TryGetComponent<Structure>(out var component) && _currentStructureToRepair != component)
        {
            _currentStructureToRepair = component.GetComponent<HealthComponent>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!(collision is BoxCollider2D))
        {
            return;
        }

        //removing from repair
        if (collision.TryGetComponent<Structure>(out var component) && _currentStructureToRepair == component)
        {
            _currentStructureToRepair = null;
        }
    }
}
