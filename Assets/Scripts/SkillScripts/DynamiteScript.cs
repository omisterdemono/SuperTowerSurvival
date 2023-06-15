using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteScript : NetworkBehaviour
{
    [SyncVar] private float _damage;
    [SerializeField] private GameObject _explosionPrefab;
    public float Damage { set => _damage = value; }

    private List<Collider2D> _colliders;

    private void Start()
    {
        _colliders = new List<Collider2D>();
        Invoke("DestroyMine", 5);
    }

    [Command(requiresAuthority = false)]
    private void DestroyMine()
    {
        foreach (var item in _colliders)
        {
            if (item.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
            {
                healthComponent.Damage(_damage);
            }
        }
        NetworkServer.Destroy(gameObject);
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(explosion);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        _colliders.Add(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            _colliders.Remove(collision);
    }
}
