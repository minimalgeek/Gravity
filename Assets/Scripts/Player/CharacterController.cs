using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class CharacterController : MonoBehaviour
{

    private Rigidbody2D rb;

    public float jumpForce = 10f;
	public float moveSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
		rb.AddForce(moveDirection*moveSpeed, ForceMode2D.Force);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
