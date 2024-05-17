using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : NetworkBehaviour
{
    [SerializeField] StatusEffect _effect;
    [SerializeField] Collider2D _mainCollider;
    [SerializeField] Collider2D _passiveAura;
    EffectComponent _effectComponent;

    private void Start()
    {
        _effectComponent = GetComponent<EffectComponent>();
        if (_effectComponent)
        {
            _effectComponent.ApplyEffect(_effect);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(collision is BoxCollider2D))
            return;

        EffectComponent effectComponent = collision.GetComponentInParent<EffectComponent>();
        if (effectComponent && collision!= _mainCollider && !_mainCollider.IsTouching(collision) && collision.tag == "HitBox")
        {
            effectComponent.ApplyEffect(_effect);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!(collision is BoxCollider2D))
            return;

        EffectComponent effectComponent = collision.GetComponentInParent<EffectComponent>();
        if (effectComponent && !_passiveAura.IsTouching(collision) && collision.tag == "HitBox")
        {
            effectComponent.RemoveEffect(_effect);
        }
    }

    public void PowerUp(int points)
    {
        _effect.Value *= 1.1f * points;
    }
}
