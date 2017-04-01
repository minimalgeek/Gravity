using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using DG.Tweening;
using Gamelogic.Extensions;



[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class CombinedController : GLMonoBehaviour
{
    public enum Facing
    {
        Left, Right
    }

    public enum State
    {
        Grounded, WasGrounded, Falling, PerformsJump, Hanging, Climbing
    }


    #region Variables and properties

    private State state = State.Falling;

    private Facing facing = Facing.Right;

    // Speeds
    [Header("Walking")]
    public float walkSpeed = 12.0f;

    private float maxVelocityChange = 10.0f;


    // Jumping
    [Header("Jumping")]
    public bool canJump = true;
    public float jumpSpeed = 12f;
    public float jumpTimeout = 0.07f;
    public float jumpCooldownTime = 0.1f;
    public float jumpClearanceTime = 0.05f;

    private bool jumpFlag = false;
    private float jumpPressTime;
    private float jumpTime;
    private bool isJumping = false;


    // Ground detection
    [Header("Ground Detection")]
    public CollisionSinkingDetector groundDetector;

    private bool grounded = false;
    public bool Grounded { get { return grounded; } }
    private Vector2 groundVelocity;
    private CapsuleCollider2D capsule;
    private Rigidbody2D rb;
    private bool sinkingSuspended = true;
    //private Vector2 groundNormal;

    // Climbing
    [Header("Climbing")]
    public bool canClimb = true;
    public CollisionSinkingDetector climbUpperDetector;
    public CollisionSinkingDetector climbLowerDetector;
    public Transform climbDestination;
    public float climbingTime = 0.5f;

    private int upperDetectionCount, lowerDetectionCount;

    // Interaction
    [Header("Interaction")]
    [SerializeField]
    private Transform itemHoldingPoint;
    public Transform ItemHoldingTransform { get { return itemHoldingPoint; } }

    #endregion Variables and properties

    #region Unity event functions

    /// <summary>
    /// Use for initialization
    /// </summary>
    void Awake()
    {
        Assert.IsNotNull(groundDetector);
        Assert.IsNotNull(climbUpperDetector);
        Assert.IsNotNull(climbLowerDetector);
        Assert.IsNotNull(climbDestination);
        Assert.IsNotNull(itemHoldingPoint);

        capsule = GetComponent<CapsuleCollider2D>();
        Assert.IsNotNull(capsule);
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
        rb.freezeRotation = true;
        groundDetector = GetComponentInChildren<CollisionSinkingDetector>();
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        groundDetector.TriggerStay += OnSinkingStay;

        climbUpperDetector.TriggerEnter += ((o) => upperDetectionCount++);
        climbUpperDetector.TriggerLeave += ((o) => upperDetectionCount--);

        climbLowerDetector.TriggerEnter += ((o) => lowerDetectionCount++);
        climbLowerDetector.TriggerLeave += ((o) => lowerDetectionCount--);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Cache the input
        if (Input.GetAxis("Jump") > 0 && canJump && Time.time - jumpTime > jumpCooldownTime)
        {
            jumpFlag = true;
            jumpPressTime = Time.time;
        }
        if (jumpFlag && Time.time - jumpPressTime > jumpTimeout)
        {
            jumpFlag = false;
        }
    }

    /// <summary>
    /// Update for physics
    /// </summary>
    void FixedUpdate()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), 0);// Input.GetAxis("Vertical"));
        Facing prevFacing = facing;
        facing = inputVector.x < 0 ? Facing.Left : (inputVector.x > 0) ? Facing.Right : prevFacing;
        if (facing != prevFacing) Flip();


        // State machine transition handling
        switch (state)
        {
            case State.Grounded:
                if (jumpFlag)
                {
                    DoJump();
                    jumpFlag = false;
                    jumpTime = Time.time;
                    state = State.PerformsJump;
                }
                else if (!grounded)
                    state = State.WasGrounded;
                break;

            case State.WasGrounded:
                if (!grounded) state = State.Falling;
                else if (jumpFlag)
                {
                    DoJump();
                    jumpFlag = false;
                    jumpTime = Time.time;
                    state = State.PerformsJump;
                }
                else state = State.Grounded;
                break;

            case State.Falling:
                if (grounded)
                {
                    DoLand();
                    state = State.Grounded;
                }
                else if (canClimb && upperDetectionCount == 0 && lowerDetectionCount > 0)
                {
                    DoHang();
                    state = State.Hanging;
                }
                break;

            case State.PerformsJump:
                if (Time.time - jumpTime > jumpClearanceTime)
                {
                    if (grounded) state = State.Grounded;
                    else state = State.Falling;
                }
                break;

            case State.Hanging:
                if (jumpFlag)
                {
                    DoClimb();
                    jumpFlag = false;
                    state = State.Climbing;
                }
                break;

            case State.Climbing:
                // Waiting for animation completion
                break;

            default:
                break;
        }
        grounded = false;


        // State machine perstistent state handling
        switch (state)
        {
            case State.Grounded:
                {
                    // Apply a force that attempts to reach our target velocity
                    Vector2 velocityChange = CalculateVelocityChange(inputVector);
                    rb.AddForce(velocityChange * rb.mass, ForceMode2D.Impulse);

                    break;
                }

            case State.WasGrounded:
                // Cannot be reached
                break;

            case State.Falling:
                // Physics does all
                break;

            case State.PerformsJump:
                // Physics does all (and waiting for ground clearance)
                break;

            case State.Hanging:
                // Nothing happens while hanging
                break;

            case State.Climbing:
                // Waiting for animation completion
                break;
            default:
                break;
        }

        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(transform.position.x, -transform.position.y) * Mathf.Rad2Deg, Vector3.forward);
    }

    void DoLand()
    {
        rb.velocity = (rb.velocity + Vector2.Reflect(rb.velocity, transform.right)) * 0.5f;
    }

    void DoJump()
    {
        rb.velocity = rb.velocity - (Vector2)Vector3.Normalize(rb.position) * jumpSpeed;// new Vector2(rb.velocity.x, rb.velocity.y + CalculateJumpVerticalSpeed());
        sinkingSuspended = true;
        groundDetector.ColliderEnabled = false;
    }

    void DoHang()
    {
        rb.simulated = false;
        rb.velocity = Vector2.zero;
    }

    void DoClimb()
    {
        //physicsCollider.enabled = false; // Hm.
        transform.DOMove(climbDestination.position, climbingTime, false).OnComplete(() =>
        {
            //physicsCollider.enabled = true;
            rb.simulated = true;
            state = State.Falling;
        });
    }

    void OnSinkingStay(Collider2D other)
    {
        grounded = true && !sinkingSuspended;
    }

    //void OnSinkingLeave(Collider2D other)
    //{
    //    sinkingSuspended = false;
    //    sinkingDetector.ColliderEnabled = true;
    //}

    // Unparent if we are no longer standing on our parent
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform == transform.parent)
            transform.parent = null;
    }

    // If there are collisions check if the character is grounded
    void OnCollisionStay2D(Collision2D col)
    {
        TrackGrounded(col);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        TrackGrounded(col);
    }

    #endregion Unity event functions

    #region Methods

    // From the user input calculate using the set up speeds the velocity change
    private Vector2 CalculateVelocityChange(Vector2 inputVector)
    {
        // Calculate how fast we should be moving
        var relativeVelocity = (Vector2)transform.TransformDirection(inputVector);
        relativeVelocity *= walkSpeed;

        // Calcualte the delta velocity
        var currRelativeVelocity = rb.velocity - groundVelocity;
        var velocityChange = relativeVelocity - currRelativeVelocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);

        return velocityChange;
    }

    // From the user input calculate using the set up speeds the velocity change
    private Vector2 CalculateVelocityChange2(Vector2 inputVector)
    {
        // Olvashatóan:
        //Vector3 currentVelocity = rb.velocity;
        //Vector3 currentStep = currentVelocity * Time.fixedDeltaTime;
        //Vector3 expectedTangential = transform.right * inputVector.x * walkSpeed;
        //Vector3 expectedTangentialCorrectedStep = Vector3.Normalize(transform.position + expectedTangential * Time.fixedDeltaTime) * transform.position.magnitude - transform.position;
        //Vector3 expectedCorrectedStep = expectedTangentialCorrectedStep + transform.up * inputVector.y * walkSpeed;
        //Vector3 velocityChange = (expectedCorrectedStep - currentStep) / Time.fixedDeltaTime;
        //return velocityChange;

        // Ugyanez olvashatatlanul:
        return (Vector3.Normalize(transform.position + transform.right * inputVector.x * walkSpeed * Time.fixedDeltaTime) * transform.position.magnitude
            - transform.position + transform.up * inputVector.y * walkSpeed)
            / Time.fixedDeltaTime
            - (Vector3)rb.velocity;
    }

    // Check if the base of the capsule is colliding to track if it's grounded
    private void TrackGrounded(Collision2D collision)
    {
        if (!isJumping)
        {
            var minRadius = capsule.transform.position.magnitude + capsule.size.y * 0.5f - capsule.size.x * 0.45f;// capsule.bounds. + capsule.size.x * .9f;
            //groundNormal = Vector2.zero;
            foreach (var contact in collision.contacts)
            {
                if (contact.point.magnitude > minRadius)
                {
                    if (isKinematic(collision))
                    {
                        // Get the ground velocity and we parent to it
                        groundVelocity = collision.rigidbody.velocity;
                        transform.parent = collision.transform;
                    }
                    else if (isStatic(collision))
                    {
                        // Just parent to it since it's static
                        transform.parent = collision.transform;
                    }
                    else
                    {
                        // We are standing over a dinamic object,
                        // set the groundVelocity to Zero to avoid jiggers and extreme accelerations
                        groundVelocity = Vector2.zero;
                    }

                    // Esta en el suelo
                    grounded = true;
                    sinkingSuspended = false;
                    groundDetector.ColliderEnabled = true;

                    //groundNormal = contact.normal;
                    break;
                }
            }
        }
    }

    private bool isKinematic(Collision2D collision)
    {
        return isKinematic(collision.transform);
    }

    private bool isKinematic(Transform transform)
    {
        Rigidbody trrb = transform.GetComponent<Rigidbody>();
        return trrb != null && trrb.isKinematic;
    }

    private bool isStatic(Collision2D collision)
    {
        return isStatic(collision.transform);
    }

    private bool isStatic(Transform transform)
    {
        return transform.gameObject.isStatic;
    }

    #endregion Methods



    public Facing GetFacingDirection()
    {
        return facing;
    }

    public Vector3 GetFacingVector()
    {
        return (facing == Facing.Right) ? transform.right : -transform.right;
    }

    public void Flip()
    {
        transform.FlipX();
    }

    public void SetState(State newState)
    {
        switch (newState)
        {
            case State.Grounded:
                break;
            case State.WasGrounded:
                break;
            case State.Falling:
                jumpFlag = false;
                break;
            case State.PerformsJump:
                break;
            case State.Hanging:
                break;
            case State.Climbing:
                break;
            default:
                break;
        }
        state = newState;
    }

    public void SetLocalVelocity(Vector2 newVelocity)
    {
        rb.velocity = transform.TransformDirection(newVelocity);
    }
}