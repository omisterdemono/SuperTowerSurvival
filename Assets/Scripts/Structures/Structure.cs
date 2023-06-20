using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : NetworkBehaviour, IBuildable
{
    private HealthComponent _healthComponent;
    private SpriteRenderer _spriteRenderer;

    [SyncVar] private Vector3 _spawnPosition;
    public bool IsBuilt { get; set; } = false;
    public Vector3 SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }

    private void Start()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        var parent = GameObject.FindGameObjectWithTag("StructuresTilemap").transform;
        transform.parent = parent;
        transform.localPosition = SpawnPosition;
    }

    public void Init()
    {
        _healthComponent.CurrentHealth = 1;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        _healthComponent.OnCurrentHealthChanged += Build;
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
}
