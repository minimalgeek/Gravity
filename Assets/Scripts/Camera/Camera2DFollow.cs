using System;
using UnityEngine;
using Gamelogic.Extensions;
using DG.Tweening;

public class Camera2DFollow : GLMonoBehaviour
{
    public Transform target;
    private PolarCharacterController characterController;
    public float damping = 1;
    public float rotationSpeed = 3;
    public float lookAheadValue = 2f;
    public float lookAboveValue = 2f;

    private Vector3 currentVelocity, currentRotationVelocity;
    private float zOffset;

    // Use this for initialization
    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER);
            target = player.transform;
            characterController = PolarCharacterController.Instance;
        }
        zOffset = (transform.position - target.position).z;
        transform.parent = null;
    }


    // Update is called once per frame
    private void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        CalculatePosition();
        CalculateRotation();
    }

    private void CalculatePosition()
    {
        Vector3 facingDir = Vector3.zero;

        if (characterController)
        {
            facingDir = characterController.GetFacingVector();
        }

        Vector3 aheadTargetPos = target.position + Vector3.forward * zOffset +  facingDir * lookAheadValue + target.up * lookAboveValue;
        transform.position = aheadTargetPos;

        // TODO: rewrite this stuff from scratch

        //transform.DOMove(aheadTargetPos, damping);
        //transform.position = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);
    }

    private void CalculateRotation()
    {
        //Quaternion newRotation = Quaternion.Slerp(transform.rotation, target.rotation, damping);
        transform.rotation = target.rotation;
        //transform.DORotate(target.rotation.eulerAngles, damping);
    }
}
