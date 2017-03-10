using UnityEngine;

public class CrudeGroundDetector : MonoBehaviour
{
    public bool grounded = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("LevelEditor"))
            grounded = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("LevelEditor"))
            grounded = false;
    }
}
