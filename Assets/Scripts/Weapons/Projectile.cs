using Mirror;
using System;
using UnityEngine;
using System.Linq;

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

    public float Damage { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //for friendly fire - now only destroys bullet, but can damage in the future
        var possibleCharacter = collision.GetComponent<Character>();
        if (possibleCharacter != null)
        {
            throw new NotImplementedException("Not implemented cause of player destroyal on server");
        }

        //do not forget about walls xd
        if (collision.CompareTag("Structure"))
        {
            throw new NotImplementedException("Not implemented cause of structure destroyal on server");
        }

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
}
