using Mirror;
using UnityEngine;

public class Structure : NetworkBehaviour, IBuildable
{
    public bool IsBeingPlaced { get; private set; } = true;
    public bool NoObstaclesUnder { get; private set; } = true;
    public bool CanBePlaced { get; private set; } = true;
    public bool IsBuilt { get; set; } = false;
    public Vector3 SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }

    [SyncVar] private Vector3 _spawnPosition;

    protected HealthComponent _healthComponent;
    protected SpriteRenderer _spriteRenderer;

    private const string _parentTag = "StructuresTilemap";
    private const float _minimalAlpha = 0.3f;

    public void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        var parent = GameObject.FindGameObjectWithTag(_parentTag).transform;
        transform.parent = parent;

        _spriteRenderer.sortingOrder += 1;
        _healthComponent.OnDeath += GetDestroyed;
    }

    public void Init()
    {
        IsBeingPlaced = false;
        _spriteRenderer.sortingOrder -= 1;

        transform.localPosition = SpawnPosition;

        _healthComponent.OnCurrentHealthChanged += Build;
        _healthComponent.ChangeHealth(1.0f);
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= Build;
    }

    public void Build()
    {
        if (_healthComponent.CurrentHealth >= _healthComponent.MaxHealth)
        {
            IsBuilt = true;
            _healthComponent.OnCurrentHealthChanged -= Build;
        }

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.b, _spriteRenderer.color.g, BuildAlpha);
    }

    private float BuildAlpha => _healthComponent.CurrentHealth / _healthComponent.MaxHealth + _minimalAlpha;

    public void Update()
    {
        if (!IsBuilt)
        {
            return;
        }
    }

    /// <summary>
    /// This method is used to get state from outside.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangePlacementState(bool newState)
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        CanBePlaced = NoObstaclesUnder && newState;
        _spriteRenderer.color = CanBePlaced ? Color.green : Color.red;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //temporary code with boxcollider2d, should be removed
        if (IsBeingPlaced && collision.GetComponent<BoxCollider2D>() != null)
        {
            NoObstaclesUnder = false;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //temporary code with boxcollider2d, should be removed
        if (IsBeingPlaced && collision.GetComponent<BoxCollider2D>() != null)
        {
            NoObstaclesUnder = true;
        }
    }

    [Command(requiresAuthority = false)]
    private void GetDestroyed()
    {
        NetworkServer.Destroy(gameObject);
    }
}
