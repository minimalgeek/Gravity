using System;
using UnityEngine;
using Gamelogic.Extensions;

[RequireComponent(typeof(Camera))]
public class Camera2DFollowFix : GLMonoBehaviour
{
    public Transform target;
    public float yOffset = 0.3f;
    
    private float zOffset;
    private Camera cam;
    private float zoomMultiplier = 1.3f;

    // Use this for initialization
    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER).transform;
        }
        cam = GetComponent<Camera>();
        zOffset = (transform.position - target.position).z;
        transform.parent = null;
    }


    // Update is called once per frame
    private void Update()
    {
        if (!target || !cam)
        {
            return;
        }

        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            cam.orthographicSize *= Mathf.Pow(zoomMultiplier, -zoomDelta);
        }

        transform.rotation = target.transform.rotation;
        transform.localPosition = new Vector3(target.position.x, target.position.y, zOffset) + target.transform.up * 0.3f * cam.orthographicSize;
    }
}
