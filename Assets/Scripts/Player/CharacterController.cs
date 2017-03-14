using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.Assertions;
using DG.Tweening;

public class CharacterController : GLMonoBehaviour
{
    public float jumpForce = 10f;
    public float moveSpeed = 5f;
    public float slowDownSpeed = 2f;
    public CollisionDetector groundDetector;
    public CollisionDetector climbUpperDetector;
    public CollisionDetector climbLowerDetector;
    public Transform toClimbRelativePos;
    private Rigidbody2D rb;
    private bool grounded;
    private bool upperDetected, lowerDetected, isClimbing; // all climbing related booleans

    void Awake()
    {
        Assert.IsNotNull(groundDetector);
        Assert.IsNotNull(climbUpperDetector);
        Assert.IsNotNull(climbLowerDetector);
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
    }

    void Start()
    {
        groundDetector.TriggerStay += (() => grounded = true);
        groundDetector.TriggerLeave += (() => grounded = false);

        climbUpperDetector.TriggerStay += (() => upperDetected = true);
        climbUpperDetector.TriggerLeave += (() => upperDetected = false);

        climbLowerDetector.TriggerStay += (() => lowerDetected = true);
        climbLowerDetector.TriggerLeave += (() => lowerDetected = false);

    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // moving
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                Vector2 localVelocity = GetLocalVelocity();
                localVelocity = Vector2.Lerp(localVelocity, Vector2.zero, Time.deltaTime * slowDownSpeed);
                SetLocalVelocity(localVelocity);
            }
            else
            {
                SpeedUpCharacter();
            }

            // jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(this.transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        else if (!upperDetected && lowerDetected)
        {
            // climbing
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isClimbing = true;
            }
            if (!isClimbing)
            {
                SetLocalVelocity(Vector2.zero);
            } else {
                rb.DOMove(toClimbRelativePos.position, 1f, false);
            }
        }
        else
        {
            isClimbing = false;
        }
    }

    private Vector2 GetLocalVelocity()
    {
        return transform.InverseTransformDirection(rb.velocity);
    }

    private void SetLocalVelocity(Vector2 newVelocity)
    {
        rb.velocity = transform.TransformDirection(newVelocity);
    }

    private void SpeedUpCharacter()
    {
        Vector2 localVelocity = GetLocalVelocity();
        localVelocity.x = Input.GetAxis("Horizontal") * moveSpeed;
        SetLocalVelocity(localVelocity);
    }
}
