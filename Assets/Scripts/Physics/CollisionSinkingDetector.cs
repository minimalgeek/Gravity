using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class CollisionSinkingDetector : GLMonoBehaviour
{
    public delegate void TriggerAction(Collider2D other);
    public event TriggerAction TriggerEnter;
    public event TriggerAction TriggerStay;
    public event TriggerAction TriggerLeave;

    private bool colliderEnabled;
    public bool ColliderEnabled
    {
        get { return colliderEnabled; }
        set
        {
            colliderEnabled = value;
            foreach (var coll in GetComponents<Collider2D>())
                coll.enabled = colliderEnabled;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (TriggerEnter != null)
            TriggerEnter(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (TriggerStay != null)
            TriggerStay(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (TriggerLeave != null)
            TriggerLeave(other);
    }
}
