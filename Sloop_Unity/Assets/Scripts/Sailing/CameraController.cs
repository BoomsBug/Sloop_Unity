using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private Transform rain;
    void Update()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Math.Abs(scrollDelta) > 0f)
        {
            mainCamera.orthographicSize -= scrollDelta * scrollSensitivity;

            rain.localScale = new Vector2(mainCamera.orthographicSize/2 ,mainCamera.orthographicSize/2);
        }

        //constrain zoom
        if (mainCamera.orthographicSize < minZoom)
            mainCamera.orthographicSize = minZoom;
            
        if (mainCamera.orthographicSize > maxZoom)
            mainCamera.orthographicSize = maxZoom;
    }
}
