using UnityEngine;
using System.Collections;

public class AmbientRotator : MonoBehaviour
{
    public UniformRotationField field;

    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = field.Omega.magnitude * Mathf.Rad2Deg;
    }

    // Update is called once per frame
    void Update()
    {
        rb.angularVelocity = field.Omega.magnitude * Mathf.Rad2Deg;
    }

    void FixedUpdate()
    {
        rb.angularVelocity = field.Omega.magnitude * Mathf.Rad2Deg;
    }
}
