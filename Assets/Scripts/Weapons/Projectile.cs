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
    [SerializeField][SyncVar] private float _speed;

    [SyncVar] private float _damage;
    [SyncVar] private Vector2 _direction;

    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public Vector2 Direction { get => _direction; set => _direction = value; }

    private void Start()
    {
        StartCoroutine(Fly());
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(Direction * Speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
