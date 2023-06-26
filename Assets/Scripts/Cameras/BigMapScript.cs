using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMapScript : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _camera.gameObject.SetActive(!_camera.gameObject.activeSelf);
            gameObject.GetComponent<RawImage>().enabled = _camera.gameObject.activeSelf;
        }
    }
}
