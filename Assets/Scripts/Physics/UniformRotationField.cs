using UnityEngine;

[ExecuteInEditMode]
public class UniformRotationField : MonoBehaviour
{
    [SerializeField]
    private float angularVelocity = 0.7f;
    [SerializeField]
    private float angularFrequency;
    [SerializeField]
    private float period;

    public float AngularVelocity
    {
        get { return angularVelocity; }
        set
        {
            angularVelocity = value;
            angularFrequency = angularVelocity / (2 * Mathf.PI);
            period = 1f / angularFrequency;
            Recalculate();
        }
    }
    public float AngularFrequency
    {
        get { return angularFrequency; }
        set
        {
            angularFrequency = value;
            angularVelocity = 2 * Mathf.PI * angularFrequency;
            period = 1 / angularFrequency;
            Recalculate();
        }
    }
    public float Period
    {
        get { return period; }
        set
        {
            period = value;
            angularFrequency = 1f / period;
            angularVelocity = 2 * Mathf.PI * angularFrequency;
            Recalculate();
        }
    }


    //public float surfaceGravity = 9.81f;
    //public float radius = 20.0f;

    private float centrifugalFactor;
    private float tangentialVelocity;
    private Vector3 omega;
    private Vector3 omega2;

    public Vector3 Omega { get { return omega; } }

    //void Update()
    //{
    //    centrifugalFactor = surfaceGravity / radius;
    //    tangentialVelocity = Mathf.Sqrt(surfaceGravity * radius);
    //    omega = Vector3.forward * Mathf.Sqrt(centrifugalFactor);
    //    omega2 = omega * 2;
    //}

    void Start()
    {
        AngularVelocity = 0.7f;
        Recalculate();
    }

    public void Recalculate()
    {
        omega = Vector3.forward * angularVelocity;
        omega2 = omega * 2;
        centrifugalFactor = angularVelocity * angularVelocity;
    }

    void FixedUpdate()
    {
        Rigidbody2D[] rigidBodies = (Rigidbody2D[])FindObjectsOfType(typeof(Rigidbody2D));

        foreach (Rigidbody2D rb in rigidBodies)
        {
            if (rb.simulated) Apply(rb);
        }
    }

    void Apply(Rigidbody2D rb)
    {
        rb.AddForce((Vector3.Cross(omega2, rb.velocity) + rb.transform.position * centrifugalFactor) * rb.mass, ForceMode2D.Force);
    }

    public float GetTangentialSpeed(float radius)
    {
        //return tangentialVelocity * radius / this.radius; // Mathf.Sqrt(surfaceGravity / atRadius) * radius;
        //TODO
        Debug.LogWarning("TODO: Tangential Speed/Velocity");
        return 0;
    }

    //public Vector2 GetTangentialVelocity(Vector2 pos)
    //{
    //    return 
    //}
}
