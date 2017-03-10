using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class CharacterController : GLMonoBehaviour
{
    public float jumpForce = 10f;
    public float moveSpeed = 5f;
    public float slowDownSpeed = 2f;
    private Rigidbody2D rb;
    private CollisionDetector groundController;
    private bool grounded;

    public void SetJumpForce(float force)
    {
        this.jumpForce = force;
    }

    public void SetMoveSpeed(float speed)
    {
        this.moveSpeed = speed;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundController = GetComponentInChildren<CollisionDetector>();
    }

    void OnEnable()
    {
        groundController.TriggerStay += (() => grounded = true);
        groundController.TriggerLeave += (() => grounded = false);
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                Vector2 localVelocity = transform.InverseTransformDirection(rb.velocity);
                localVelocity = Vector2.Lerp(localVelocity, Vector2.zero, Time.deltaTime * slowDownSpeed);
                rb.velocity = transform.TransformDirection(localVelocity);
            }
            else
            {
                Vector2 localVelocity = transform.InverseTransformDirection(rb.velocity);
                localVelocity.x = Input.GetAxis("Horizontal") * moveSpeed;
                rb.velocity = transform.TransformDirection(localVelocity);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(this.transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}
