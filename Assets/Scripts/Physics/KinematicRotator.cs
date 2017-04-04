using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class KinematicRotator : MonoBehaviour {

	public float angularVelocity;
	private Rigidbody2D rb;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		Assert.IsNotNull(rb);
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		rb.angularVelocity = angularVelocity;
	}
}
