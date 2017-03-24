using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.Assertions;
using DG.Tweening;

public class PolarCharacterController : GLMonoBehaviour
{
    public float jumpSpeed = 12f;
    public float jumpTimeout = 0.1f;
    public CollisionDetector groundDetector;
    public CollisionDetector climbUpperDetector;
    public CollisionDetector climbLowerDetector;
    public Transform toClimbRelativePos;
    public GameObject itemHoldingPoint;
    private Rigidbody2D rb;
    private CapsuleCollider2D physicsCollider;
    private MirrorToInput facingController;
    private float actualHorizontalSpeed;
    private CharacterMover mover;
    private bool grounded;
    private bool upperDetected, lowerDetected, isHanging; // all climbing related booleans
    private bool canJump;
    private bool jumpFlag;
    private float jumpPressTime;

    public bool IsGrounded { get { return grounded; } }
    public float ActualHorizontalSpeed { set { actualHorizontalSpeed = value; } }

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
        mover = GetComponentInParent<CharacterMover>();
        Assert.IsNotNull(mover);
    }

    void Start()
    {
        groundDetector.TriggerStay += (() => { grounded = true; canJump = true; SetLocalXVelocityToZero(); Debug.Log("Touchdown!"); });
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
            //Debug.Log("\thanging");
            isHanging = true;
            canJump = true;
        }
        else
        {
            isHanging = false;
        }
        if (Input.GetAxis("Jump") > 0 && canJump)
        {
            jumpFlag = true;
            jumpPressTime = Time.time;
            Debug.LogWarning("JUMP!!");
        }
    }


    void FixedUpdate()
    {
        if (grounded)
        {
            //Debug.Log(Time.frameCount + " G");
            // jumping
            if (jumpFlag)
            {
                jumpFlag = false;
                Debug.LogWarning(Time.frameCount + "    J");
                rb.AddForce(transform.up * jumpSpeed * rb.mass, ForceMode2D.Impulse);
                Vector2 locVel = GetLocalVelocity();
                locVel.x = actualHorizontalSpeed;
                SetLocalVelocity(locVel);
            }
            mover.ApplyMotion();
        }
        else if (isHanging)
        {
            //Debug.Log(Time.frameCount + " H");
            rb.simulated = false;
            SetLocalVelocity(Vector2.zero);

            // climbing
            if (jumpFlag)
            {
                jumpFlag = false;
                canJump = false;
                Debug.LogWarning(Time.frameCount + "    J");
                physicsCollider.enabled = false; // Hm.
                transform.DOLocalMove(toClimbRelativePos.position, 1f, false).OnComplete(() =>
                {
                    physicsCollider.enabled = true;
                    rb.simulated = true;
                });
            }
        }
        else
        {
            //Debug.Log(Time.frameCount + " A");
            if (jumpFlag && Time.time - jumpPressTime > jumpTimeout) jumpFlag = false;
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
