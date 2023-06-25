using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoisonousGasScript : NetworkBehaviour
{
    [SerializeField][SyncVar] private float _damage;
    [SerializeField][SyncVar] private float _damageRate;
    [SerializeField][SyncVar] private float _workTime;
    [SerializeField][SyncVar] private float _slowDownModifier;
    [SerializeField][SyncVar] private Vector2 _throwTo;
    [SerializeField][SyncVar] private Vector2 _throwFrom;
    [SerializeField] private ParticleSystem _smoke1;
    [SerializeField] private ParticleSystem _smoke2;
    private float duration = 1f;

    private float elapsedTime = 0f;
    public float Damage { set => _damage = value; }
    public float DamageRate { get => _damageRate; set => _damageRate = value; }
    public float WorkTime { get => _workTime; set => _workTime = value; }
    public float SlowDownModifier { get => _slowDownModifier; set => _slowDownModifier = value; }
    public Vector2 ThrowTo { get => _throwTo; set => _throwTo = value; }

    private List<Collider2D> _colliders;

    private void Start()
    {
        _throwFrom = transform.position;
        _colliders = new List<Collider2D>();
        Invoke("DestroyGas", _workTime);
        InvokeRepeating("DealDamage", 1, DamageRate);
    }

    private void FixedUpdate()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.position = Vector2.Lerp(_throwFrom, _throwTo, t);
        }
        else
        {
            if (!_smoke1.isPlaying)
            {
                _smoke1.Play();
            }

            if (!_smoke2.isPlaying)
            {
                _smoke2.Play();
            }

            GetComponent<CircleCollider2D>().enabled = true;
            
        }
    }

    [Command(requiresAuthority = false)]
    private void DestroyGas()
    {
        CancelInvoke("DealDamage");
        NetworkServer.Destroy(gameObject);
    }

    private void DealDamage()
    {
        foreach (var collider in _colliders)
        {
            if (!collider.CompareTag("Player") && elapsedTime>=duration)
            {
                if (collider.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
                {
                    healthComponent.Damage(_damage);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && elapsedTime >= duration)
        {
            _colliders.Add(collision);
            
            if(collision.TryGetComponent<MovementComponent>(out MovementComponent movementComponent))
            {
                movementComponent.Speed *= SlowDownModifier;
            }
        }   
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            _colliders.Remove(collision);

            if (collision.TryGetComponent<MovementComponent>(out MovementComponent movementComponent))
            {
                movementComponent.Speed /= SlowDownModifier;
            }
        }
    }
}
