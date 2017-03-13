using System;
using UnityEngine;
using Gamelogic.Extensions;

public class Camera2DFollow : GLMonoBehaviour
{
    public Transform target;
    public float damping = 1;
    public float rotationSpeed = 3;
    public float lookAheadValue = 2f;
    [Tooltip("Lookahead is recalculated, when the rotation angle exceeds this threshold")]
    public float lookAheadThreshold = 10f;

    private Vector3 currentVelocity;
    private float zOffset;
    private float previousRotationZ;

    // Use this for initialization
    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER).transform;
        }
        zOffset = (transform.position - target.position).z;
        previousRotationZ = GetRotationZEuler();
        transform.parent = null;
    }


    // Update is called once per frame
    private void Update()
    {
        if (!target)
        {
            return;
        }

        CalculateLookaheadPosition();
        CalculateRotation();
    }

    private void CalculateLookaheadPosition()
    {
        Vector3 facingDir;

        if (previousRotationZ < GetRotationZEuler())
        {
            facingDir = target.right;
        }
        else
        {
            facingDir = target.right * -1;
        }

        Vector3 aheadTargetPos = target.position + Vector3.forward * zOffset + lookAheadValue * facingDir;
        transform.position = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);

        if (Mathf.Abs(GetRotationZEuler() - previousRotationZ) > lookAheadThreshold)
        {
            previousRotationZ = GetRotationZEuler();
        }
    }

    private float GetRotationZEuler()
    {
        return target.rotation.eulerAngles.z + 180f;
    }

    private void CalculateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, target.rotation.eulerAngles.z), Time.deltaTime * rotationSpeed);
    }
}
