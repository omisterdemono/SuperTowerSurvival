using Mirror;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// This class is responsible for attacking enemy entities,
/// but if needed, can be extended to attack something else.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private string[] _desiredTargets;
    [SerializeField] private Type _owner;

    [SerializeField] private float _timeToFlyInSeconds;
    [SerializeField] private float _speed;

    public float Damage { get; set; }

    [SyncVar] public Vector2 Direction;

    private void Start()
    {
        StartCoroutine(Fly());
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(Direction * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //for friendly fire - now only destroys bullet, but can damage in the future
        //var possibleCharacter = collision.GetComponent<Character>();
        //if (possibleCharacter != null)
        //{
        //    throw new NotImplementedException("Not implemented cause of player destroyal on server");
        //}

        //do not forget about walls xd
        //if (collision.CompareTag("Structure"))
        //{
        //    if (!_desiredTargets.Contains("Structure"))
        //    {
        //        DestroyProjectile();
        //    }

        //    throw new NotImplementedException("Not implemented because class structure is needed");
        //}

        //hitting all desired targets
        foreach (var target in _desiredTargets)
        {
            var component = collision.GetComponent(target);

            if(component != null) 
            {
                component.GetComponent<IDamageAble>().TakeDamage(Damage);
                return;
            }
        }
    }

    private IEnumerator Fly()
    {
        yield return new WaitForSeconds(_timeToFlyInSeconds);

        DestroyProjectile();
    }

    [Command(requiresAuthority = false)]
    private void DestroyProjectile()
    {
        NetworkServer.Destroy(gameObject);
    }
}
