using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class CollisionDetector : GLMonoBehaviour
{

    public delegate void TriggerAction();
    public event TriggerAction TriggerStay;
    public event TriggerAction TriggerLeave;

    [TooltipAttribute("Layers to trigger on")]
    public List<LayerMask> triggeringLayers = new List<LayerMask>();

    void OnTriggerStay2D(Collider2D other)
    {

        if (IsTriggered(other))
        {
            if (TriggerStay != null)
                TriggerStay();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsTriggered(other))
        {
            if (TriggerLeave != null)
                TriggerLeave();
        }
    }

    private bool IsTriggered(Collider2D other) {
        LayerMask otherLayer = (1 << other.gameObject.layer);
        return triggeringLayers.Contains(otherLayer);
    }
}
