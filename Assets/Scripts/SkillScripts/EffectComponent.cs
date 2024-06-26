using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;
using System;

public class EffectComponent : NetworkBehaviour
{
    [SyncVar] private List<StatusEffect> _effects = new List<StatusEffect>();
    [SerializeField] BoxCollider2D _mainCollider;
    public List<StatusEffect> Effects { get => _effects; set => _effects = value; }
    public BoxCollider2D MainCollider { get => _mainCollider; set => _mainCollider = value; }
    //public static Action<GameObject> OnEffectRemoved;
    public Action OnEffectRemoved;

    [Server]
    public void ApplyEffect(StatusEffect effect)
    {
        _effects.Add(effect);
        if (effect.HandleTime > 0)
        {
            StartCoroutine(RemoveTimedEffect(effect, effect.HandleTime));
        }
        else
        {
            StartCoroutine(HandleEffect(effect));
        }
        _effects = _effects.OrderBy(e => e.ApplyType).ToList();
    }

    private IEnumerator HandleEffect(StatusEffect effect)
    {
        while (_effects.Where(e => e.Name == effect.Name).Count() >= 1)
        {
            yield return new WaitForSeconds(1);
            if (effect.EffectType == EEffect.Damage)
            {
                GetComponent<HealthComponent>().Damage(effect.Value);
            }
            if (effect.EffectType == EEffect.Heal)
            {
                GetComponent<HealthComponent>().Heal(effect.Value);
            }
        }
    }

    private IEnumerator RemoveTimedEffect(StatusEffect effect, float time)
    {
        float timePassed = 0;
        while (timePassed < time)
        {
            yield return new WaitForSeconds(1);
            timePassed++;
            if (effect.EffectType == EEffect.Damage)
            {
                GetComponent<HealthComponent>().Damage(effect.Value);
            }
            if (effect.EffectType == EEffect.Heal)
            {
                GetComponent<HealthComponent>().Heal(effect.Value);
            }
        }
        //yield return new WaitForSeconds(time);
        RemoveEffect(effect);
        //OnEffectRemoved.Invoke();
    }
    [ClientRpc]
    public void RemoveEffect(StatusEffect effect)
    {
        if (_effects.Count > 0)
            _effects.Remove(_effects.Where(x => x.Name == effect.Name).First());
    }

    public float GetValue(float currVal, StatusEffect effect)
    {
        switch (effect.ApplyType)
        {
            case EApplyType.Add:
                currVal += effect.Value;
                break;
            case EApplyType.Set:
                currVal = effect.Value;
                break;
            case EApplyType.Percent:
                currVal *= effect.Value;
                break;
        }
        return currVal;
    }
}
