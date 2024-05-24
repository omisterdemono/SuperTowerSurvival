using System;
using Infrastructure.UI;
using Mirror;
using UIScripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Components
{
    public class HealthComponent : NetworkBehaviour
    {
        public enum EntityType
        {
            Default,
            Player,
            Zombie,
            Skeleton,
            Structure
        }

        public Action OnCurrentHealthChanged;
        public Action OnDeath;
        public Action OnHit;
        public static event EventHandler OnEntityHit;


        [SerializeField] private float _maxHealth;
        [FormerlySerializedAs("_type")] [FormerlySerializedAs("type")] [SerializeField] public EntityType _entityType;

        [SyncVar(hook = nameof(CurrentHealthHook))] private float _currentHealth;

        [SerializeField] private HealthBarUI _healthBar;

        [SyncVar] private float _hpRatio;
        private CanvasGroup _canvasGroupHB;
        private Image _imageHP;

        public float MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
            }
        }
        public float CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            set
            {
                //_currentHealth = value;
                ChangeHealth(value);
            }
        }

        public void Heal(float healHP)
        {
            if (CurrentHealth + healHP >= MaxHealth)
            {
                CurrentHealth = MaxHealth;
                return;
            }
            CurrentHealth += healHP;
        }

        [ClientRpc]
        public void Damage(float damageHP)
        {
            if (CurrentHealth <= damageHP)
            {
                CurrentHealth = 0;

                OnDeath?.Invoke();
                return;
            }
            CurrentHealth -= damageHP;
            OnEntityHit?.Invoke(this, EventArgs.Empty);
            OnHit?.Invoke();
        }
        
        private void CurrentHealthHook(float oldValue, float newValue)
        {
            _hpRatio = _currentHealth / _maxHealth;
            if (_healthBar != null)
            {
                _healthBar.SetHealthInPercent(_hpRatio);
            }
            OnCurrentHealthChanged?.Invoke();
        }

        [Command(requiresAuthority = false)]
        public void ChangeHealth(float health)
        {
            _currentHealth = health;
        }

        [Server]
        private void InitHealth() 
        {
            ChangeHealth(MaxHealth);
        }

        private void Start()
        {
            InitHealth();

            if (_entityType == EntityType.Player && isOwned)
            {
                _healthBar = FindObjectOfType<PlayerHealthbarUI>();
            }

            if (_healthBar == null)
            {
                _healthBar = GetComponentInChildren<HealthBarUI>();
            }
        }
    }
}