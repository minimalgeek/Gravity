using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.Assertions;
using DG.Tweening;

public class PolarCharacterController : Singleton<PolarCharacterController>
{
    public float jumpSpeed = 12f;
    public float climbingTime = 1f;
    public CollisionDetector groundDetector;
    public CollisionDetector climbUpperDetector;
    public CollisionDetector climbLowerDetector;
    public Transform toClimbRelativePos;
    public GameObject itemHoldingPoint;
    public Transform throwCross;
    private Rigidbody2D rb;
    private CapsuleCollider2D physicsCollider;
    private MirrorToInput facingController;
    private float actualHorizontalSpeed;
    private CharacterMover mover;
    private bool grounded;
    private bool upperDetected, lowerDetected, isHanging; // all climbing related booleans
    private bool jumpFlag;
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

    /* it's needed, because there are so much change on groundDetector stay and leave, 
       it triggers the local velocity setters too often */
    private IEnumerator ApplyHorizontalSpeedALittleLater() {
        yield return new WaitForSeconds(0.005f);
        SetLocalXVelocityToActualHorizontalSpeed();
    }
    void Start()
    {
        groundDetector.TriggerStay += (() =>
        {
            grounded = true;
            StopAllCoroutines();
            SetLocalXVelocityToZero();
        });
        groundDetector.TriggerLeave += (() => {
            grounded = false;
            StartCoroutine(ApplyHorizontalSpeedALittleLater());
            //SetLocalXVelocityToActualHorizontalSpeed();
        });

        climbUpperDetector.TriggerStay += (() => upperDetected = true);
        climbUpperDetector.TriggerLeave += (() => upperDetected = false);

        climbLowerDetector.TriggerStay += (() => lowerDetected = true);
        climbLowerDetector.TriggerLeave += (() => lowerDetected = false);
    }

    void Update()
    {
        isHanging = !upperDetected && lowerDetected;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpFlag = true;
        }
    }

    
    void FixedUpdate()
    {
        if (grounded)
        {
            if (jumpFlag)
            {
                jumpFlag = false;
                SetLocalXVelocityToActualHorizontalSpeed();
                rb.AddForce(transform.up * jumpSpeed * rb.mass, ForceMode2D.Impulse);
            } else {
                mover.ApplyMotion();
            }
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
                Debug.LogWarning(Time.frameCount + "    J");
                physicsCollider.enabled = false; // Hm.
                transform.DOMove(toClimbRelativePos.position, climbingTime, false).OnComplete(() =>
                {
                    physicsCollider.enabled = true;
                    rb.simulated = true;
                });
            }
        }
        else
        {
            jumpFlag = false;
            rb.simulated = true; // let him fall, if he was hanging before
        }
    }

    public Facing GetFacingDirection()
    {
        return facingController.facing;
    }

    public Vector3 GetFacingVector()
    {
        return GetFacingDirection() == Facing.LEFT ? transform.right * -1 : transform.right;
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

    private void SetLocalXVelocityToActualHorizontalSpeed()
    {
        Vector2 locVel = GetLocalVelocity();
        locVel.x = actualHorizontalSpeed;
        SetLocalVelocity(locVel);
    }
}
