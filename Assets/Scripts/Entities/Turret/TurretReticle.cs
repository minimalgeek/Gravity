using UnityEngine;

public class TurretReticle : MonoBehaviour
{
    LineRenderer line;

    void Start()
    {
        line = transform.parent.gameObject.GetComponent<LineRenderer>();
    }

    public void OnMouseDrag()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        line.SetPosition(1, transform.position);
    }
}
