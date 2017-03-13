using System;
using UnityEngine;
using Gamelogic.Extensions;

public class Camera2DFollow : GLMonoBehaviour
{
    public Transform target;
    public float damping = 1;
    public float rotationSpeed = 3;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;

    private float zOffset;
    private Vector3 lastTargetPosition;
    private Vector3 currentVelocity;
    private Vector3 lookAheadPos;

    // Use this for initialization
    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER).transform;
        }
        lastTargetPosition = target.position;
        zOffset = (transform.position - target.position).z;
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
        // only update lookahead pos if accelerating or changed direction
        Vector3 diff = (target.position - lastTargetPosition);
        float moveDelta = diff.x + diff.y;
        bool updateLookAheadTarget = Mathf.Abs(moveDelta) > lookAheadMoveThreshold;

        if (updateLookAheadTarget)
        {
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(moveDelta);
        }
        else
        {
            lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        }

        Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * zOffset;
        transform.position = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);

        lastTargetPosition = target.position;
    }

    private void CalculateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, target.rotation.eulerAngles.z), Time.deltaTime * rotationSpeed);
    }
}
