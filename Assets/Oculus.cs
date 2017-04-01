using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oculus : MonoBehaviour {

	Rigidbody2D parentRB;
	CombinedController player;

	// Use this for initialization
	void Start () {
		parentRB = GetComponentInParent<Rigidbody2D>();
		player = GetComponentInParent<CombinedController>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward,(Vector3)parentRB.velocity + player.GetFacingVector()), 0.1f);
	}
}
