using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : NetworkBehaviour, IBuildable
{
    [SerializeField] private string _parentTag;
    public bool IsBeingPlaced { get; private set; } = true;
    public bool CanBePlaced { get; private set; } = true;
    public bool IsBuilt { get; set; } = false;
    public Vector3 SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }
    
    [SyncVar] private Vector3 _spawnPosition;

    private HealthComponent _healthComponent;
    private SpriteRenderer _spriteRenderer;


    private void Start()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        var parent = GameObject.FindGameObjectWithTag("StructuresTilemap").transform;
        transform.parent = parent;
        transform.localPosition = SpawnPosition;

        _spriteRenderer.sortingOrder += 1;
    }

    public void Init()
    {
        IsBeingPlaced = false;
        _spriteRenderer.sortingOrder -= 1;

        _healthComponent.CurrentHealth = 1;
        _healthComponent.OnCurrentHealthChanged += Build;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        _spriteRenderer.color = Color.white;
    }

    public void Build()
    {
        if (_healthComponent.CurrentHealth == _healthComponent.MaxHealth)
        {
            IsBuilt = true;
            _healthComponent.OnCurrentHealthChanged -= Build;
        }

        _spriteRenderer.material.color = new Color(0, 0, 0, _healthComponent.CurrentHealth / _healthComponent.MaxHealth);
    }

    private void Update()
    {
        if (!IsBuilt)
        {
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //temporary code with boxcollider2d, should be removed
        if (IsBeingPlaced && collision.GetComponent<BoxCollider2D>() != null)
        {
            ChangePlacementState(false);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //temporary code with boxcollider2d, should be removed
        if (IsBeingPlaced && collision.GetComponent<BoxCollider2D>() != null)
        {
            ChangePlacementState(true);
        }
    }

    public void ChangePlacementState(bool newState)
    {
        if (newState == CanBePlaced)
        {
            return;
        }

        CanBePlaced = newState;
        _spriteRenderer.color = CanBePlaced ? Color.green : Color.red;
    }
}
