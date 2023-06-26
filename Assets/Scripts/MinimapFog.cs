using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFog : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fog"))
        {
            Destroy(collision.gameObject);
        }
    }
}
