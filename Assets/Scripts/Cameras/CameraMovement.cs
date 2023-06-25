using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 _offset = new(0f, 0f, -10f);
    private float _smoothTime = 0.5f;
    private Vector3 _velocity = Vector3.zero;
    public Transform Target = null;

    void FixedUpdate()
    {
        if (Target)
        {
            Vector3 targetPosition = Target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
        }
    }
}
