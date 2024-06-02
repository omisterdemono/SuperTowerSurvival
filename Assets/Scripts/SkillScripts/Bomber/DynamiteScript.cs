using Mirror;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;

public class DynamiteScript : NetworkBehaviour
{
    [SyncVar] private float _damage;
    [SerializeField] private GameObject _explosionPrefab;

    public float Damage
    {
        set => _damage = value;
    }

    private List<Collider2D> _colliders = new List<Collider2D>();

    private void Start()
    {
        Invoke("DestroyMine", 5);
    }

    [Command(requiresAuthority = false)]
    private void DestroyMine()
    {
        for (int i = 0; i <= _colliders.Count-1; i++)
        {
            if (_colliders[i].TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
            {
                _colliders.Remove(_colliders[i]);
                healthComponent.Damage(_damage);
            }
            if (_colliders.Count==0)
            {
                break;
            }
        }

        NetworkServer.Destroy(gameObject);
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(explosion);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("HitBox") && !collision.CompareTag("Player") && collision is BoxCollider2D)
            _colliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("HitBox") && !collision.CompareTag("Player") && collision is BoxCollider2D)
            _colliders.Remove(collision);
    }
}