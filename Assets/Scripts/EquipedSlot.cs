using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EquipedSlot : MonoBehaviour
{
    private SpriteRenderer _itemSpriteRenderer;

    private void Awake()
    {
        _itemSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        _itemSpriteRenderer.flipY = !(transform.eulerAngles.z <= 90.0f || transform.eulerAngles.z >= 270.0f);
    }

    public void Rotate(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

}
