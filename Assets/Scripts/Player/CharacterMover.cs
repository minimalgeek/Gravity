using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterMover : MonoBehaviour
{

    public float moveSpeed = 10f;
    public CharacterController characterController;
	private float horizontalDelta;

	public float HorizontalDelta {
		get {
			return horizontalDelta;
		}
	}

	void Start()
	{
		characterController = GetComponentInChildren<CharacterController>();
	}

    void Update()
    {
        if (transform.position != Vector3.zero) transform.position = Vector3.zero;
    }

	void FixedUpdate()
	{
		if (characterController.IsGrounded)
        {
        	horizontalDelta = Input.GetAxis("Horizontal") * moveSpeed;
			characterController.ActualHorizontalSpeed = horizontalDelta;
            transform.Rotate(Vector3.forward * horizontalDelta * Time.fixedDeltaTime, Space.Self);
        }
	}
}
