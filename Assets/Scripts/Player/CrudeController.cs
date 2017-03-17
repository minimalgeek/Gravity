using UnityEngine;
using UnityEngine.Assertions;

public class CrudeController : MonoBehaviour
{
    public float horizontalAcceleration = 8;
    public float maxTSpeed = 8;
    public float jumpSpeed = 8;
    public float deceleration = 20;

    private Rigidbody2D rb;
    private LineRenderer velociLine;

    private Camera cam;
    private float zoomMultiplier = 1.3f;
    private bool following = true;

    public CollisionDetector groundDetector;
    private bool isGrounded;

    public CollisionDetector groundSinkDetector;
    private bool isSunken;

    public bool intertiaTrace = true;
    private Transform background;
    private Object tracer;

    private bool doJump;
    private bool doLeftJump;
    private bool doRightJump;
    private float tangentialAxis;

    void Awake()
    {
        Assert.IsNotNull(groundDetector);
        Assert.IsNotNull(groundSinkDetector);
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
    }

    void Start()
    {
        groundDetector.TriggerStay += OnGroundDetectorTriggerStay;
        groundDetector.TriggerLeave += OnGroundDetectorTriggerLeave;

        groundSinkDetector.TriggerStay += OnGroundSinkStay;
        groundSinkDetector.TriggerLeave += OnGroundSinkLeave;

        rb = GetComponentInChildren<Rigidbody2D>();
        Debug.Log(groundDetector);
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        velociLine = GetComponentInChildren<LineRenderer>();

        try
        {
            background = GameObject.Find("Background").transform;
        }
        catch { }
        try
        {
            tracer = Resources.Load("Tracer", typeof(GameObject));
        }
        catch { }
    }

    public void OnGroundDetectorTriggerStay()
    {
        Debug.LogWarning("     GS");
        isGrounded = true;

        if (!doJump && !doLeftJump && !doRightJump)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void OnGroundDetectorTriggerLeave()
    {
        Debug.LogWarning("       GL");
        isGrounded = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void OnGroundSinkStay()
    {
        Debug.LogWarning("S");
        isSunken = true;
    }

    public void OnGroundSinkLeave()
    {
        Debug.LogWarning("  L");
        isSunken = false;
    }

    void Update()
    {
        //if (isGrounded)
        {
            rb.rotation = Mathf.Atan2(rb.position.x, -rb.position.y) * Mathf.Rad2Deg;
            if (following) cam.transform.rotation = rb.transform.rotation;
        }

        tangentialAxis = Input.GetAxis("Horizontal");

        doJump |= Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W);
        doLeftJump |= Input.GetKeyDown(KeyCode.Q);
        doRightJump |= Input.GetKeyDown(KeyCode.E);
    }

    void LateUpdate()
    {
        // Zoom
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            cam.orthographicSize *= Mathf.Pow(zoomMultiplier, -Input.GetAxis("Mouse ScrollWheel"));
            //Vector3 camPos = cam.transform.localPosition;
            //camPos.y = 0.3f * cam.orthographicSize;
            //cam.transform.localPosition = camPos;
        }
        if (following) cam.transform.localPosition = new Vector3(rb.position.x, /*0.3f * cam.orthographicSize +*/ rb.position.y, cam.transform.localPosition.z) + transform.up * 0.3f * cam.orthographicSize;

        // Kamera követés ki/be
        if (Input.GetMouseButtonDown(2))
        {
            following = !following;
        }

        velociLine.SetPosition(0, (Vector3)rb.position + Vector3.back);
        velociLine.SetPosition(1, (Vector3)(rb.position + rb.velocity) + Vector3.back);
    }

    void FixedUpdate()
    {
        //isGrounded = groundDetector.grounded;
        if (!isGrounded) rb.bodyType = RigidbodyType2D.Dynamic;
        Debug.Log(isGrounded + "  " + rb.isKinematic + "  s: " + isSunken);

        if (Input.GetKey(KeyCode.S))
            rb.velocity = -rb.transform.up * Mathf.Abs(Vector2.Dot(rb.velocity, rb.transform.up));
        else
        {
            if (isGrounded)
            {
                if (isSunken)
                    rb.MovePosition(rb.position + (Vector2)rb.transform.up * 0.05f);

                float currentTSpeed = Vector2.Dot(rb.velocity, rb.transform.right);
                if (tangentialAxis > 0)
                {
                    if (currentTSpeed < maxTSpeed)
                    {
                        if (rb.isKinematic)
                            rb.velocity = rb.transform.right * maxTSpeed;
                        else
                            rb.AddRelativeForce(Vector2.right * tangentialAxis * rb.mass * horizontalAcceleration);
                    }
                }
                else if (tangentialAxis < 0)
                {
                    if (currentTSpeed > -maxTSpeed)
                    {
                        if (rb.isKinematic)
                            rb.velocity = rb.transform.right * (-maxTSpeed);
                        else
                            rb.AddRelativeForce(Vector2.right * tangentialAxis * rb.mass * horizontalAcceleration);
                    }
                }
                else
                {
                    //rb.velocity = Vector3.Normalize(rb.velocity) * (currentTSpeed - Time.deltaTime * deceleration);
                    rb.velocity = rb.velocity * Mathf.Exp(-Time.deltaTime * deceleration);
                }

                if (doJump)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.AddRelativeForce(Vector2.up * jumpSpeed * rb.mass, ForceMode2D.Impulse);
                    doJump = false;
                }
                if (doLeftJump)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.AddRelativeForce((Vector2.left * maxTSpeed + Vector2.up * jumpSpeed) * rb.mass, ForceMode2D.Impulse);
                    doLeftJump = false;
                }
                if (doRightJump)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.AddRelativeForce((Vector2.right * maxTSpeed + Vector2.up * jumpSpeed) * rb.mass, ForceMode2D.Impulse);
                    doRightJump = false;
                }
            }
        }

        //if (isGrounded)
        rb.rotation = Mathf.Atan2(rb.position.x, -rb.position.y) * Mathf.Rad2Deg;

        if (!isGrounded && intertiaTrace)
            Instantiate(tracer, rb.position, rb.transform.rotation, background);
    }

    //void OnCollisionEnter2D(Collision2D coll)
    //{
    //    Debug.Log(coll.gameObject.name + " " + coll.relativeVelocity + " " + coll.contacts);
    //}

    void OnGUI()
    {
        Vector3 textPos = cam.WorldToScreenPoint(rb.position);
        textPos.y = cam.pixelHeight - textPos.y;
        GUI.Label(new Rect(textPos, new Vector2(100, 30)), ((Vector2)transform.InverseTransformDirection(rb.velocity)).ToString());
    }
}
