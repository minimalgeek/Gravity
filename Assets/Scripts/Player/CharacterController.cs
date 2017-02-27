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
        //worldUp.Normalize();
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
        rb.AddRelativeForce(moveDirection*moveSpeed, ForceMode2D.Force);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(this.transform.up * jumpForce, ForceMode2D.Impulse);
        }

        this.transform.up = worldCenter.transform.position - this.transform.position;
    }

    public void SetJumpForce(float force) {
        this.jumpForce = force;
    }

    public void SetMoveSpeed(float speed) {
        this.moveSpeed = speed;
    }
}
