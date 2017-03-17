using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class CollisionSinkingDetector : GLMonoBehaviour
{

    public delegate void TriggerAction(Collider2D other);
    public event TriggerAction TriggerStay;
    public event TriggerAction TriggerLeave;

    public LayerMask groundLayer;

    private bool colliderEnabled;
    public bool ColliderEnabled
    {
        get { return colliderEnabled; }
        set
        {
            colliderEnabled = value;
            foreach (var coll in GetComponents<Collider2D>())
            {
                coll.enabled = colliderEnabled;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer) == groundLayer)
        {
            if (TriggerStay != null)
                TriggerStay(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer) == groundLayer)
        {
            if (TriggerLeave != null)
                TriggerLeave(other);
        }
    }
}
