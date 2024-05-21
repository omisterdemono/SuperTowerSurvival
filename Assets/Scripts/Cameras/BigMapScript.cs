using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMapScript : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CanvasGroup _panel;
    Vector3 mouseClickPos;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _camera.gameObject.SetActive(!_camera.gameObject.activeSelf);
            _panel.alpha =  _camera.gameObject.activeSelf ? 1f : 0f;
            _panel.blocksRaycasts =  _camera.gameObject.activeSelf ? true : false;
            _camera.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void LateUpdate()
    {
        if (gameObject.GetComponent<RawImage>().enabled)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                if ((_camera.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * 20) <= 100 && (_camera.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * 20) >= 10)
                _camera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 20;
            }
            if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
            {
                Vector3 newCameraPos;
                newCameraPos.x = Input.GetAxisRaw("Mouse X");
                newCameraPos.y = Input.GetAxisRaw("Mouse Y");
                newCameraPos.z = 0;
                _camera.transform.localPosition -= newCameraPos * _camera.orthographicSize/26;
            }
        }
    }
}
