using UnityEngine;
using System.Collections;

public class OrbitStarter : MonoBehaviour
{
    public UniformRotationField field;

    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody2D>().velocity = new Vector3(0, -field.GetTangentialSpeed(Mathf.Abs(transform.position.x)), 0);
	}
}
