using System;
using Infrastructure.UI;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class HealthComponent : NetworkBehaviour
    {
        public enum Type
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
        [SerializeField] public Type type;

        [SyncVar(hook = nameof(CurrentHealthHook))] private float _currentHealth;

        private void CurrentHealthHook(float oldValue, float newValue)
        {
            _hpRatio = _currentHealth / _maxHealth;
            if (_imageHP != null)
            {
                _imageHP.fillAmount = _hpRatio;
            }
            OnCurrentHealthChanged?.Invoke();
        }

        [SerializeField] private GameObject _healthBar;

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

        [Command(requiresAuthority = false)]
        public void ChangeHealth(float health)
        {
            _currentHealth = health;
        }

        private void InitHealth() 
        {
            if (!isServer)
            {
                return;
            }
            
            ChangeHealth(MaxHealth);
        }

        void Start()
        {
            //if (isLocalPlayer)
            //{
            //    _currentHealth = _maxHealth;
            //}
            InitHealth();

            if (_healthBar != null)
            {
                _canvasGroupHB = _healthBar.GetComponentInChildren<CanvasGroup>();
                _imageHP = _canvasGroupHB.transform.GetChild(1).GetComponentInChildren<Image>();
            }

            if (type == Type.Player && isOwned)
            {
                var healthbar = FindObjectOfType<PlayerHealthBarUI>();
                _imageHP = healthbar.transform.GetChild(0).GetComponentInChildren<Image>();
            }
            
            Debug.Log($"[Game init] {gameObject.name} Health Component Start");
        }
    }
}