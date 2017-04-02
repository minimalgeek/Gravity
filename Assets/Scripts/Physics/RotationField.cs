using UnityEngine;
using Gamelogic.Extensions;

[ExecuteInEditMode]
public class RotationField : Singleton<RotationField>
{
    [SerializeField]
    private float angularVelocity = -0.7f;
    [SerializeField]
    private float angularFrequency;
    [SerializeField]
    private float period;


    private bool accelerating = false;
    private float targetAngularVelocity;
    private float angularAcceleration = -0.7f;

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

    void Awake()
    {
        AngularVelocity = angularVelocity;
        Recalculate();
    }

    void Start()
    {
        AngularVelocity = angularVelocity;
        Recalculate();
    }

    public void Recalculate()
    {
        omega = Vector3.back * angularVelocity;
        omega2 = omega * 2;
        centrifugalFactor = angularVelocity * angularVelocity;
    }

    void FixedUpdate()
    {
        Rigidbody2D[] rigidBodies = (Rigidbody2D[])FindObjectsOfType(typeof(Rigidbody2D));

        if (accelerating)
        {
            float difference = targetAngularVelocity - angularVelocity;
            AngularVelocity += angularAcceleration * Time.fixedDeltaTime;
            if (difference * (targetAngularVelocity - angularVelocity) < 0)
            {
                AngularVelocity = targetAngularVelocity;
                accelerating = false;
            }


            foreach (Rigidbody2D rb in rigidBodies)
            {
                if (rb.simulated)
                {
                    rb.AddForce(
                        (
                            Vector3.Cross(omega2, rb.velocity)
                            + rb.transform.position * centrifugalFactor
                            + Vector3.Cross(Vector3.back * angularAcceleration, rb.transform.position)
                        )
                        * rb.mass,
                        ForceMode2D.Force);
                    rb.AddTorque(
                        -angularAcceleration
                        * rb.inertia,
                        ForceMode2D.Force);
                }
            }
        }
        else
        {
            foreach (Rigidbody2D rb in rigidBodies)
            {
                if (rb.simulated) rb.AddForce((Vector3.Cross(omega2, rb.velocity) + rb.transform.position * centrifugalFactor) * rb.mass, ForceMode2D.Force);
            }
        }
    }

    public float GetTangentialSpeed(float radius)
    {
        return angularVelocity * radius;
    }

    public Vector2 GetTangentialVelocity(Vector2 pos)
    {
        return Vector3.Cross(omega, pos);
    }


    public void AffectParticles(ref ParticleSystem.Particle[] particles, int aliveCount)
    {
        for (int i = 0; i < aliveCount; ++i)
        {
            particles[i].velocity += (Vector3.Cross(omega2, particles[i].velocity) + particles[i].position * centrifugalFactor) * Time.fixedDeltaTime;
        }
    }


    public void AffectParticles(ref ParticleSystem.Particle[] particles, int aliveCount, float factor)
    {
        for (int i = 0; i < aliveCount; ++i)
        {
            particles[i].velocity += (Vector3.Cross(omega2, particles[i].velocity) + particles[i].position * centrifugalFactor) * Time.fixedDeltaTime * factor;
        }
    }


    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = new Color(0, 0, 1, 0.75F);
        Gizmos.DrawLine(Vector2.one, -Vector2.one);
        Gizmos.DrawLine(new Vector2(1, -1), new Vector2(-1, 1));
        int direction = (int)Mathf.Sign(angularFrequency);
        if (direction != 0)
        {
            Gizmos.DrawLine(new Vector2(-1.5f, -2), new Vector2(1.5f, -2));
            Gizmos.DrawLine(new Vector2(-1.5f * direction, -2f), new Vector2(-1 * direction, -1.8f));
            Gizmos.DrawLine(new Vector2(-1.5f * direction, -2f), new Vector2(-1 * direction, -2.2f));
        }
    }

    public void AccelerateTo(float targetAngularVelocity, float absAngularAcceleration)
    {
        if (targetAngularVelocity > angularVelocity)
            this.angularAcceleration = Mathf.Abs(absAngularAcceleration);
        else
            this.angularAcceleration = -Mathf.Abs(absAngularAcceleration);
        this.targetAngularVelocity = targetAngularVelocity;
        accelerating = true;
    }

    public void AccelerateToWithDuration(float targetAngularVelocity, float duration)
    {
        this.angularAcceleration = (targetAngularVelocity - angularVelocity) / duration;
        this.targetAngularVelocity = targetAngularVelocity;
        accelerating = true;
    }
}
