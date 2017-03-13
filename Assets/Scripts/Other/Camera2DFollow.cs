using System;
using UnityEngine;
using Gamelogic.Extensions;

public class Camera2DFollow : GLMonoBehaviour
{
    public Transform target;
    public float damping = 1;
    public float rotationSpeed = 3;
    public float lookAheadValue = 2f;

    private Vector3 currentVelocity;
    private float zOffset;
    private float previousHorizontalInput;

    // Use this for initialization
    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER).transform;
        }
        zOffset = (transform.position - target.position).z;
        previousHorizontalInput = 0f;
        transform.parent = null;
    }


    // Update is called once per frame
    private void Update()
    {
        if (!target)
        {
            return;
        }

        CalculatePosition();
        CalculateRotation();
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            previousHorizontalInput = Input.GetAxisRaw("Horizontal");
        }
    }

    private void CalculatePosition()
    {
        Vector3 facingDir;

        if (previousHorizontalInput > 0)
        {
            facingDir = target.right;
        }
        else
        {
            facingDir = target.right * -1;
        }

        Vector3 aheadTargetPos = target.position + Vector3.forward * zOffset + lookAheadValue * facingDir;
        transform.position = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);
    }

    private float GetRotationZEuler()
    {
        return target.rotation.eulerAngles.z + 180f;
    }

    private void CalculateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationSpeed);
    }
}
