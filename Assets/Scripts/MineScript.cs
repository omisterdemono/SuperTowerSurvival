using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MineScript : NetworkBehaviour
{
    private float _damage;
    private bool _explode = false;
    [SerializeField] private GameObject _explosionPrefab;

    public float Damage { set => _damage = value; }

    [Command(requiresAuthority = false)]
    private void DestroyMine()
    {
        NetworkServer.Destroy(gameObject);
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(explosion);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Vector2.Distance((Vector2)collision.transform.position, (Vector2)transform.position) <= 1f 
            && !collision.CompareTag("Player") 
            && !_explode 
            && collision.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
        {
            _explode = true;
            GetComponentInChildren<ParticleSystem>().Play();
            healthComponent.Damage(_damage);
            DestroyMine();
        }
    }
}
