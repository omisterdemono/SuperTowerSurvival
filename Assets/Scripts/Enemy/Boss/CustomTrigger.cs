using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider2D> EnteredTrigger;
    public event System.Action<Collider2D> ExitedTrigger;
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnteredTrigger?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ExitedTrigger?.Invoke(other);
    }
}
