using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class TurretReticle : MonoBehaviour, IDragHandler
{
    LineRenderer line;

    void Start()
    {
        line = transform.parent.gameObject.GetComponent<LineRenderer>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position);
        line.SetPosition(1, transform.position);
    }
}
