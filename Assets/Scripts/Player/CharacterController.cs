using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.Assertions;
using DG.Tweening;

public class CharacterController : GLMonoBehaviour
{
    public float jumpSpeed = 10f;
    public float moveSpeed = 5f;
    public float slowDownSpeed = 2f;
    public CollisionDetector groundDetector;
    public CollisionDetector climbUpperDetector;
    public CollisionDetector climbLowerDetector;
    public Transform toClimbRelativePos;
    public GameObject itemHoldingPoint;
    private Rigidbody2D rb;
    private CircleCollider2D physicsCollider;
    private MirrorToInput facingController;
    private bool grounded;
    private bool upperDetected, lowerDetected, isClimbing; // all climbing related booleans

    void Awake()
    {
        Assert.IsNotNull(groundDetector);
        Assert.IsNotNull(climbUpperDetector);
        Assert.IsNotNull(climbLowerDetector);
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
        physicsCollider = GetComponent<CircleCollider2D>();
        Assert.IsNotNull(physicsCollider);
        facingController = GetComponentInChildren<MirrorToInput>();
        Assert.IsNotNull(facingController);
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
                Decelerate();
            }
            else
            {
                Accelerate();
            }

            // jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(this.transform.up * jumpSpeed * rb.mass, ForceMode2D.Impulse);
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
            }
            else
            {
                physicsCollider.enabled = false;
                rb.DOMove(toClimbRelativePos.position, 1f, false).OnComplete(() => physicsCollider.enabled = true);
            }
        }
        else
        {
            isClimbing = false;
        }
    }

    public Facing GetFacingDirection()
    {
        return facingController.facing;
    }

    private Vector2 GetLocalVelocity()
    {
        return transform.InverseTransformDirection(rb.velocity);
    }

    public void SetLocalVelocity(Vector2 newVelocity)
    {
        rb.velocity = transform.TransformDirection(newVelocity);
    }

    private void Accelerate()
    {
        Vector2 localVelocity = GetLocalVelocity();
        localVelocity.x = Input.GetAxis("Horizontal") * moveSpeed;
        SetLocalVelocity(localVelocity);
    }

    private void Decelerate()
    {
        Vector2 localVelocity = GetLocalVelocity();
        localVelocity = Vector2.Lerp(localVelocity, Vector2.zero, Time.deltaTime * slowDownSpeed);
        SetLocalVelocity(localVelocity);
    }
}
