using Mirror;
using UnityEngine;

public class Structure : NetworkBehaviour, IBuildable
{
    [SerializeField] private Sprite[] _rotationSprites;
    [SerializeField] private GameObject _projectile;
    public bool IsBeingPlaced { get; private set; } = true;
    public bool NoObstaclesUnder { get; private set; } = true;
    public bool CanBePlaced { get; private set; } = true;
    public bool IsBuilt { get; set; } = false;
    public Vector3 SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }
    
    [SyncVar] private Vector3 _spawnPosition;
    [SyncVar] private int _rotateIndex;

    private HealthComponent _healthComponent;
    private SpriteRenderer _spriteRenderer;

    private Transform _currentTarget;

    private const string _parentTag = "StructuresTilemap";
    private const float _minimalAlpha = 0.3f;

    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        var parent = GameObject.FindGameObjectWithTag(_parentTag).transform;
        transform.parent = parent;

        _spriteRenderer.sortingOrder += 1;
    }

    public void Init()
    {
        IsBeingPlaced = false;
        _spriteRenderer.sortingOrder -= 1;

        transform.localPosition = SpawnPosition;

        _healthComponent.OnCurrentHealthChanged += Build;
        _healthComponent.CurrentHealth = 1.0f;
    }

    public void Build()
    {
        if (_healthComponent.CurrentHealth == _healthComponent.MaxHealth)
        {
            IsBuilt = true;
            _healthComponent.OnCurrentHealthChanged -= Build;
        }

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.b, _spriteRenderer.color.g, BuildAlpha);
    }

    private float BuildAlpha => _healthComponent.CurrentHealth / _healthComponent.MaxHealth + _minimalAlpha;

    private void Update()
    {
        if (!IsBuilt)
        {
            return;
        }

        RotateTowardsTarget();
        CountRotationIndex();
    }

    private void RotateTowardsTarget()
    {
        if (_rotateIndex == -1)
        {
            return;
        }

        _spriteRenderer.sprite = _rotationSprites[_rotateIndex];
    }

    [Server]
    private void CountRotationIndex()
    {
        if (_currentTarget == null)
        {
            return;
        }

        var direction = (_currentTarget.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = angle < 0 ? 360.0f + angle : angle;
        _rotateIndex = (int)((angle / 360.0f) * _rotationSprites.Length);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogWarning("Remove IsBuilt there");
        IsBuilt = true;

        //temporary code with boxcollider2d, should be removed
        if (IsBeingPlaced && collision.GetComponent<BoxCollider2D>() != null)
        {
            NoObstaclesUnder = false;
        }

        //handling enemies that entered the attack radius
        if (collision.TryGetComponent<Enemy>(out Enemy enemy) && _currentTarget == null)
        {
            _currentTarget = enemy.transform;
            _rotateIndex = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //temporary code with boxcollider2d, should be removed
        if (IsBeingPlaced && collision.GetComponent<BoxCollider2D>() != null)
        {
            NoObstaclesUnder = true;
        }

        //removing enemy that left the attack radius
        if (collision.TryGetComponent<Enemy>(out Enemy enemy) && enemy.transform == _currentTarget)
        {
            _currentTarget = null;
            _rotateIndex = -1;
        }
    }

    /// <summary>
    /// This method is used to get state from outside.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangePlacementState(bool newState)
    {
        if(_spriteRenderer == null)
        {
            return;
        }

        CanBePlaced = NoObstaclesUnder && newState;
        _spriteRenderer.color = CanBePlaced ? Color.green : Color.red;
    }
}
