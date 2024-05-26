using System;
using System.Collections;
using Components;
using UnityEngine;

namespace Structures
{
    public class StructureHealthComponent : HealthComponent
    {
        [SerializeField] private float _timeToHideHealthbarSeconds = 2.0f;

        private bool _healthWasChanged;
        private Coroutine _currentHealthbarHideCoroutine;

        public new void Start()
        {
            base.Start();

            _healthBar.gameObject.SetActive(false);
            OnCurrentHealthChanged += HandleHealthbar;
        }

        private void HandleHealthbar()
        {
            if (_healthWasChanged)
            {
                StopCoroutine(_currentHealthbarHideCoroutine);
                _healthWasChanged = false;
            }
            
            _healthBar.gameObject.SetActive(true);
            _healthWasChanged = true;

            _currentHealthbarHideCoroutine = StartCoroutine(HideHealthbar());
        }

        private IEnumerator HideHealthbar()
        {
            yield return new WaitForSeconds(_timeToHideHealthbarSeconds);

            _healthBar.gameObject.SetActive(false);
            _healthWasChanged = false;
        }

        private void OnMouseOver()
        {
            if (_healthWasChanged)
            {
                return;
            }
            
            _healthBar.gameObject.SetActive(true);
        }
        
        private void OnMouseExit()
        {
            if (_healthWasChanged)
            {
                return;
            }
            
            _healthBar.gameObject.SetActive(false);
        }
    }
}