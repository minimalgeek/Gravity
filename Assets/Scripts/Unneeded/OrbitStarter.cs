using UnityEngine;

public class OrbitStarter : MonoBehaviour
{
    public enum RotationMode {
        Ineratial, Corotating
    }

    public RotationMode startingRotation = RotationMode.Corotating;
    
    void Start () {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) {
            rb.velocity = RotationField.Instance.GetTangentialVelocity(rb.position);
            if (startingRotation == RotationMode.Ineratial) rb.angularVelocity = -RotationField.Instance.AngularVelocity*Mathf.Rad2Deg;
        }
	}
}
