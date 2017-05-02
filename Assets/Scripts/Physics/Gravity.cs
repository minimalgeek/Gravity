using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;

public class Gravity : Singleton<Gravity>
{
    [TooltipAttribute("Rad/sec")]
    [SerializeField]
    private float angularVelocity;
    private Rigidbody2D[] affectedObjects;

    public float AngularVelocity {
        set {
            this.angularVelocity = value;
        }
    }

    void FixedUpdate()
    {
        FindAffectedObjects();
        foreach (Rigidbody2D rb in affectedObjects)
        {
            if (rb.simulated && !rb.isKinematic)
                Attract(rb);
        }
    }

    private void FindAffectedObjects()
    {
        affectedObjects = GameObject.FindObjectsOfType<Rigidbody2D>();
    }

    private void Attract(Rigidbody2D body)
    {
        Transform t = body.transform;
        Vector3 gravityUp = t.position - transform.position;
        // Centripetal force
        Vector3 centripetalForce = body.mass * gravityUp * Mathf.Pow(angularVelocity, 2);
        Debug.DrawLine(t.position, t.position + centripetalForce, Color.red);
        body.AddForce(centripetalForce);

        // Coriolis force
        Vector3 coriolisForce = -2 * body.mass * Vector3.Cross(Vector3.forward * angularVelocity, body.velocity);
        Debug.DrawLine(t.position, t.position + coriolisForce, Color.green);
        body.AddForce(coriolisForce);
    }
}