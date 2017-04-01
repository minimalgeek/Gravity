using Gamelogic.Extensions;
using UnityEngine;

public class Oculus : GLMonoBehaviour
{

    public float juiceConstant = 0.1f;
    public bool temporalEffect = true;
    public float integrationCoefficient = 0.4f;

    Rigidbody2D parentRB;
    CombinedController player;
    UniformRotationField field;
	Transform pickup;
	CombinedController.Facing prevFacing;

    private float integrationFactor = 0;
	private float overshooter;

    // Use this for initialization
    void Start()
    {
        try
        {
            parentRB = GetComponentInParent<Rigidbody2D>();
            player = GetComponentInParent<CombinedController>();
            field = GameObject.FindGameObjectWithTag("MainForceField").GetComponent<UniformRotationField>();
			pickup = player.ItemHoldingTransform;
			prevFacing = player.GetFacingDirection();
        }
        catch { }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
			CombinedController.Facing facing = player.GetFacingDirection();
			if (prevFacing != facing) {
				transform.rotation *= Quaternion.AngleAxis(180, player.transform.up);
				prevFacing = facing;
			}

            if (temporalEffect)
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.LookRotation(Vector3.forward, (Vector3)parentRB.velocity + player.GetFacingVector()) * Quaternion.AngleAxis(Time.time * field.AngularVelocity * Mathf.Rad2Deg, Vector3.back), juiceConstant);
            else
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.LookRotation(Vector3.forward, (Vector3)parentRB.velocity + player.GetFacingVector()), juiceConstant);

            if (Input.GetAxis("Fire1") > 0 || pickup.childCount > 0)
            {
                integrationFactor += Time.deltaTime * integrationCoefficient;
            }
            else {
				overshooter = overshooter * 0.95f + integrationFactor*0.05f;
				integrationFactor -= overshooter;
			}

            transform.rotation *= Quaternion.AngleAxis(Time.time * integrationFactor, Vector3.forward);
        }
        catch { }
    }
}
