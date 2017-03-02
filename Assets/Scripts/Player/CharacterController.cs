using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class CharacterController : GLMonoBehaviour
{
    private Rigidbody2D rb;

    public float jumpForce = 10f;
    public float moveSpeed = 5f;
    public Vector3 worldUp;
    private Gravity worldCenter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        worldCenter = FindObjectOfType(typeof(Gravity)) as Gravity;
    }

    void FixedUpdate()
    {
        Vector2 moveVector = new Vector2(Input.GetAxis("Horizontal"), 0) * moveSpeed;
        rb.AddRelativeForce(moveVector, ForceMode2D.Force);

        /*
        Vector2 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = Input.GetAxis("Horizontal") * moveSpeed;
        rb.velocity = transform.TransformDirection(localVelocity);
        */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(this.transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void SetJumpForce(float force)
    {
        this.jumpForce = force;
    }

    public void SetMoveSpeed(float speed)
    {
        this.moveSpeed = speed;
    }

}
