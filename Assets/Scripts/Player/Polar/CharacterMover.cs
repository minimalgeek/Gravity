using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterMover : MonoBehaviour
{
    public float moveSpeed = 12f;
    public PolarCharacterController characterController;

    private float horizontalDelta;
    public float HorizontalDelta { get { return horizontalDelta; } }

    void Start()
    {
        characterController = GetComponentInChildren<PolarCharacterController>();
    }

    void Update()
    {
        if (transform.position != Vector3.zero)
        {
            Vector3 difference = transform.position;
            transform.position = Vector3.zero;
            //apply equal and opposite movement to each child
            foreach (Transform child in transform)
            {
                child.position += difference;
            }
        }
    }

    //void FixedUpdate()
    //{
    //    if (characterController.IsGrounded)
    //    {
    //        ApplyMotion();
    //    }
    //}

    public void ApplyMotion()
    {
        horizontalDelta = Input.GetAxis("Horizontal") * moveSpeed;
        characterController.ActualHorizontalSpeed = horizontalDelta;
        transform.Rotate(Vector3.forward * horizontalDelta * Time.fixedDeltaTime * Mathf.Rad2Deg * Mathf.PI * 0.5f / characterController.transform.position.magnitude, Space.Self);
    }
}
