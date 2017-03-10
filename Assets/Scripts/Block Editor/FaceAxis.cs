using UnityEngine;

[ExecuteInEditMode]
public class FaceAxis : MonoBehaviour
{
    // Rotate object to face the axis.
    public bool autoRotate = true;

    // Update is called once per frame
    void Update()
    {
        if (autoRotate)
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(transform.position.x, -transform.position.y) * Mathf.Rad2Deg, Vector3.forward);
    }

    public static Quaternion GetRotator(Vector2 pos)
    {
        return Quaternion.AngleAxis(Mathf.Atan2(pos.x, -pos.y) * Mathf.Rad2Deg, Vector3.forward);
    }
}
