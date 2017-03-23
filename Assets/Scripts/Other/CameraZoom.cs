using System;
using UnityEngine;
using Gamelogic.Extensions;

[RequireComponent(typeof(Camera))]
public class CameraZoom : GLMonoBehaviour
{
    public float yOffset = 0.3f;
    
    private Camera cam;
    private float zoomMultiplier = 1.3f;

    // Use this for initialization
    private void Start()
    {
        cam = GetComponent<Camera>();
    }


    // Update is called once per frame
    private void Update()
    {
        if ( !cam)
        {
            return;
        }

        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            cam.orthographicSize *= Mathf.Pow(zoomMultiplier, -zoomDelta);
        }
        
        transform.localPosition = Vector3.up * yOffset * cam.orthographicSize + Vector3.zero.WithZ(transform.localPosition.z);
    }
}
