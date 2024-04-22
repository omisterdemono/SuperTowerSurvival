using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider2D> EnteredTrigger;
    public event System.Action<Collider2D> ExitedTrigger;

    public List<GameObject> colliderList = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnteredTrigger?.Invoke(other);
        if (!colliderList.Contains(other.gameObject))
        {
            colliderList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ExitedTrigger?.Invoke(other);
        if (colliderList.Contains(other.gameObject))
        {
            colliderList.Remove(other.gameObject);
        }
    }
}
