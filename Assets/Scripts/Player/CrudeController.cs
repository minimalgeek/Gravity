using UnityEngine;

public class CrudeController : MonoBehaviour
{
    public float horizontalAcceleration = 8;
    public float maxTSpeed = 8;
    public float jumpSpeed = 8;

    private Rigidbody2D rb;
    private LineRenderer velociLine;

    private Camera cam;
    private float zoomMultiplier = 1.3f;
    private bool following = true;

    private CrudeGroundDetector groundDetector;
    private bool isGrounded;

    public bool intertiaTrace = true;
    private Transform background;
    private Object tracer;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        groundDetector = GetComponentInChildren<CrudeGroundDetector>();
        //cam = GetComponentInChildren<Camera>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        velociLine = GetComponentInChildren<LineRenderer>();

        background = GameObject.Find("Background").transform;
        tracer = Resources.Load("Tracer", typeof(GameObject));
    }

    void Update()
    {
        //if (isGrounded)
        {
            rb.rotation = Mathf.Atan2(rb.position.x, -rb.position.y) * Mathf.Rad2Deg;
            if (following) cam.transform.rotation = rb.transform.rotation;
        }
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
        isGrounded = groundDetector.grounded;

        if (Input.GetKey(KeyCode.S))
            rb.velocity = -rb.transform.up * Mathf.Abs(Vector2.Dot(rb.velocity, rb.transform.up));
        else
        {
            if (isGrounded)
            {
                float currentTSpeed = Vector2.Dot(rb.velocity, rb.transform.right);
                float TAxis = Input.GetAxis("Horizontal");
                if (TAxis > 0)
                {
                    if (currentTSpeed < maxTSpeed)
                        rb.AddRelativeForce(Vector2.right * TAxis * rb.mass * horizontalAcceleration);
                }
                else if (TAxis < 0)
                {
                    if (currentTSpeed > -maxTSpeed)
                        rb.AddRelativeForce(Vector2.right * TAxis * rb.mass * horizontalAcceleration);
                }
                else
                {
                    rb.velocity = rb.velocity * Mathf.Exp(-Time.deltaTime * 2);
                }


                if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W))
                {
                    rb.AddRelativeForce(Vector2.up * jumpSpeed * rb.mass, ForceMode2D.Impulse);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    rb.AddRelativeForce((Vector2.left * maxTSpeed + Vector2.up * jumpSpeed) * rb.mass, ForceMode2D.Impulse);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    rb.AddRelativeForce((Vector2.right * maxTSpeed + Vector2.up * jumpSpeed) * rb.mass, ForceMode2D.Impulse);
                }
            }
        }

        //if (isGrounded)
            rb.rotation = Mathf.Atan2(rb.position.x, -rb.position.y) * Mathf.Rad2Deg;

        if (!isGrounded && intertiaTrace)
            Instantiate(tracer, rb.position, rb.transform.rotation, background);
    }

    void OnGUI()
    {
        Vector3 textPos = cam.WorldToScreenPoint(rb.position);
        textPos.y = cam.pixelHeight - textPos.y;
        GUI.Label(new Rect(textPos, new Vector2(100, 30)), ((Vector2)transform.InverseTransformDirection(rb.velocity)).ToString());
    }
}
