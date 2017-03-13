using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;

public class Gravity : Singleton<Gravity>
{
    [TooltipAttribute("Rotation per second")]
    public float frequency;
    private Rigidbody2D[] affectedObjects;

    // Physics
    private float angularVelocity;

    public void SetFrequency(float freq)
    {
        this.frequency = freq;
    }

    void FixedUpdate()
    {
        FindAffectedObjects();
        angularVelocity = 2 * Mathf.PI * frequency;
        foreach (Rigidbody2D rb in affectedObjects)
        {
            Attract(rb);
        }
    }

    private void FindAffectedObjects()
    {
        affectedObjects = GameObject.FindObjectsOfType<Rigidbody2D>();
    }

/*    void FixedUpdate()
    {
        angularVelocity = 2 * Mathf.PI * frequency;
        foreach (Rigidbody2D rb in affectedObjects)
        {
            Attract(rb);
        }
    }*/

    private void Attract(Rigidbody2D body)
    {
        Transform t = body.transform;
        Vector3 gravityUp = t.position - transform.position;
        // Centripetal force
        Vector3 gravityUpNormal = gravityUp;
        gravityUpNormal.Normalize();
        Vector3 centripetalForce = body.mass * gravityUp.magnitude * Mathf.Pow(angularVelocity, 2) * gravityUpNormal;
        //Vector3 centripetalForce = body.mass * gravityUp * Mathf.Pow(angularVelocity, 2);
        Debug.DrawLine(t.position, t.position + centripetalForce, Color.red);
        body.AddForce(centripetalForce);

        // Coriolis force
        Vector3 rotationVector = Vector3.forward * angularVelocity;
        Vector3 velocityVector = body.velocity;
        Vector3 coriolisForce = -2 * body.mass * Vector3.Cross(rotationVector, velocityVector);
        Debug.DrawLine(t.position, t.position + coriolisForce, Color.green);
        body.AddForce(coriolisForce);
    }
}