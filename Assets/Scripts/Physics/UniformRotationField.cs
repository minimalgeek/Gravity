using UnityEngine;
using System.Collections;

public class UniformRotationField : MonoBehaviour
{
    public float surfaceGravity = 9.81f;
    public float atRadius = 20.0f;

    private float centrifugalFactor;
    private float tangentialVelocity;
    private Vector3 omega;
    private Vector3 omega2;

    public Vector3 Omega { get { return omega; } }

    void Update()
    {
        centrifugalFactor = surfaceGravity / atRadius;
        tangentialVelocity = Mathf.Sqrt(surfaceGravity * atRadius);
        omega = Vector3.forward * Mathf.Sqrt(centrifugalFactor);
        omega2 = omega * 2;
    }

    void FixedUpdate()
    {
        Rigidbody2D[] rigidBodies = (Rigidbody2D[])GameObject.FindObjectsOfType(typeof(Rigidbody2D));

        foreach (Rigidbody2D rb in rigidBodies)
        {
            if (rb.simulated) Apply(rb);
        }
    }

    void Apply(Rigidbody2D rb)
    {
        rb.AddForce((Vector3.Cross(omega2, rb.velocity) + rb.transform.position * centrifugalFactor) * rb.mass, ForceMode2D.Force);
    }


    //void OnTriggerStay2D(Collider2D other)
    //{
    //    float onePerN = 1f / other.gameObject.GetComponents<Collider2D>().Length;
    //    Rigidbody2D rb = other.attachedRigidbody;
    //    rb.AddForce((Vector3.Cross(omega2, rb.velocity) + (other.transform.position - transform.position) * centrifugalFactor) * rb.mass * onePerN, ForceMode2D.Force);
    //}

    public float GetTangentialSpeed(float radius)
    {
        return tangentialVelocity * radius / atRadius; // Mathf.Sqrt(surfaceGravity / atRadius) * radius;
    }
}
