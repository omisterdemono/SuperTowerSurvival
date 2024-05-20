using System;
using Components;
using Mirror;
using UnityEngine;

namespace Structures
{
    public class Structure : NetworkBehaviour, IBuildable
    {
        [SerializeField] private CustomTrigger _placementChecker;
        [SerializeField] private bool _notPlaceable;
        public bool IsBeingPlaced { get; private set; } = true;
        public bool NoObstaclesUnder { get; private set; } = true;
        public bool CanBePlaced { get; private set; } = true;
        public bool IsBeingBuilt { get; set; } = false;
        public Vector3 SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }

        [SyncVar] private Vector3 _spawnPosition;

        protected HealthComponent _healthComponent;
        protected SpriteRenderer _spriteRenderer;
        private bool _isSpriteRendererNull;

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

        public void Start()
        {
            _isSpriteRendererNull = _spriteRenderer == null;

            if (!_notPlaceable)
            {
                _placementChecker.EnteredTrigger += SetNoObstaclesUnder;
                _placementChecker.ExitedTrigger += SetObstaclesUnder;
            }
        }

        public void Init()
        {
            IsBeingPlaced = false;
            _spriteRenderer.sortingOrder -= 1;

            transform.localPosition = SpawnPosition;

            _healthComponent.OnCurrentHealthChanged += Build;
            _healthComponent.CurrentHealth = 1.0f;
        }

        private void OnDestroy()
        {
            _healthComponent.OnDeath -= Build;
        }

        public void Build()
        {
            if (_healthComponent.CurrentHealth >= _healthComponent.MaxHealth)
            {
                IsBeingBuilt = true;
                _healthComponent.OnCurrentHealthChanged -= Build;
            }

            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.b, _spriteRenderer.color.g, BuildAlpha);
        }

        private float BuildAlpha => _healthComponent.CurrentHealth / _healthComponent.MaxHealth + _minimalAlpha;

        public void Update()
        {
            if (!IsBeingBuilt)
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
            if (_isSpriteRendererNull)
            {
                return;
            }

            CanBePlaced = NoObstaclesUnder && newState;
            _spriteRenderer.color = CanBePlaced ? Color.green : Color.red;
        }

        private void SetNoObstaclesUnder(Collider2D other)
        {
            if (IsBeingPlaced && other is BoxCollider2D)
            {
                NoObstaclesUnder = false;
                Debug.Log("No obstacles");
            }
        }
        
        private void SetObstaclesUnder(Collider2D other)
        {
            if (IsBeingPlaced && other is BoxCollider2D)
            {
                NoObstaclesUnder = true;
                Debug.Log("obstacles");
            }
        }

        [Command(requiresAuthority = false)]
        private void GetDestroyed()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
