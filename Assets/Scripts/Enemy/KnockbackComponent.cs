using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackComponent : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private HealthComponent health;
    //[SerializeField] private float strength = 16;
    //[SerializeField] private float delay = 0.15f;

    public UnityEvent OnBegin, OnDone;
    private void Start()
    {
        //health.OnHit += () => PlayKnockback(new Vector2(1f, 0f));
    }

    //public void PlayKnockback(Vector2 direction, )
    //{
    //    PlayKnockback
    //}
    public void PlayKnockback(Vector2 direction, float strength = 5f, float delay = 0.3f)
    {
        StopAllCoroutines();
        OnBegin?.Invoke();
        rigidbody2d.AddForce(direction * strength, ForceMode2D.Impulse);
        StartCoroutine(Reset(delay));
    }
    private IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);
        rigidbody2d.velocity = Vector3.zero;
    }
}
