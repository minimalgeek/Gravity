using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class CollisionDetector : GLMonoBehaviour
{

    public delegate void TriggerAction();
    public event TriggerAction TriggerStay;
    public event TriggerAction TriggerLeave;

    public LayerMask groundLayer;

    void OnTriggerStay2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer) == groundLayer)
        {
            TriggerStay();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer) == groundLayer)
        {
            TriggerLeave();
        }
    }
}
