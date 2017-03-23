using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.Assertions;
using DG.Tweening;

public class CharacterController : GLMonoBehaviour
{
    public float jumpSpeed = 10f;
    public CollisionDetector groundDetector;
    public CollisionDetector climbUpperDetector;
    public CollisionDetector climbLowerDetector;
    public Transform toClimbRelativePos;
    public GameObject itemHoldingPoint;
    private Rigidbody2D rb;
    private CapsuleCollider2D physicsCollider;
    private MirrorToInput facingController;
    private float actualHorizontalSpeed;
    private bool grounded;
    private bool upperDetected, lowerDetected, isHanging; // all climbing related booleans

    public bool IsGrounded
    {
        get
        {
            return grounded;
        }
    }

    public float ActualHorizontalSpeed
    {
        set
        {
            this.actualHorizontalSpeed = value;
        }
    }

    public Rigidbody2D RB
    {
        get
        {
            return rb;
        }
    }

    void Awake()
    {
        Assert.IsNotNull(groundDetector);
        Assert.IsNotNull(climbUpperDetector);
        Assert.IsNotNull(climbLowerDetector);
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
        physicsCollider = GetComponent<CapsuleCollider2D>();
        Assert.IsNotNull(physicsCollider);
        facingController = GetComponentInChildren<MirrorToInput>();
        Assert.IsNotNull(facingController);
    }

    void Start()
    {
        groundDetector.TriggerStay += (() => { grounded = true; SetLocalXVelocityToZero(); });
        groundDetector.TriggerLeave += (() => grounded = false);

        climbUpperDetector.TriggerStay += (() => upperDetected = true);
        climbUpperDetector.TriggerLeave += (() => upperDetected = false);

        climbLowerDetector.TriggerStay += (() => lowerDetected = true);
        climbLowerDetector.TriggerLeave += (() => lowerDetected = false);
    }

    void Update()
    {
        if (!upperDetected && lowerDetected)
        {
            isHanging = true;
        }
        else
        {
            isHanging = false;
        }
    }


    void FixedUpdate()
    {
        Debug.Log("FixedUpdtae");

        if (grounded)
        {
            // jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(this.transform.up * jumpSpeed * rb.mass, ForceMode2D.Impulse);
                Vector2 locVel = GetLocalVelocity();
                locVel.x = actualHorizontalSpeed;
                SetLocalVelocity(locVel);
            }
        }
        else if (isHanging)
        {
            rb.simulated = false;
            SetLocalVelocity(Vector2.zero);

            // climbing
            if (Input.GetKeyDown(KeyCode.Space))
            {
                physicsCollider.enabled = false;
                transform.DOLocalMove(toClimbRelativePos.position, 1f, false).OnComplete(() =>
                {
                    physicsCollider.enabled = true;
                    rb.simulated = true;
                });
            }
        }
    }

    public Facing GetFacingDirection()
    {
        return facingController.facing;
    }

    public Vector2 GetLocalVelocity()
    {
        return transform.InverseTransformDirection(rb.velocity);
    }

    public void SetLocalVelocity(Vector2 newVelocity)
    {
        rb.velocity = transform.TransformDirection(newVelocity);
    }

    public void SetLocalXVelocityToZero()
    {
        Vector2 localVelocity = GetLocalVelocity();
        localVelocity.x = 0;
        SetLocalVelocity(localVelocity);
    }
}
