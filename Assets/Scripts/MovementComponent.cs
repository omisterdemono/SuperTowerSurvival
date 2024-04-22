using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class MovementComponent : NetworkBehaviour
{
    private EffectComponent _effect;
    public float Speed 
    {
        set
        {
            _speed = value;
        }
        get
        {
            if (_effect)
            {
                float tmpSpeed = _speed;
                foreach (var effect in _effect.Effects)
                {
                    if (effect.EffectType == EEffect.Speed)
                    {
                        tmpSpeed = _effect.GetValue(tmpSpeed, effect);
                    }
                }
                return tmpSpeed;
            }

            return _speed;
        }
    }
    public Vector3 MovementVector
    {
        set
        {
            _movementVector = value;
        }

        get
        {
            return _movementVector;
        }
    }
    
    [SerializeField] private float _speed = 3;
    private Vector3 _movementVector;

    private void Awake()
    {
        _effect = GetComponent<EffectComponent>();
    }
    public void Move()
    {
        transform.position += _movementVector.normalized * Speed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        //Move();
    }
    void Start()
    {
        
    }
}
