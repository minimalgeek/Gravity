using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToCenterRotator : MonoBehaviour {
	
	private Gravity worldCenter;
	void Start () {
		worldCenter = FindObjectOfType(typeof(Gravity)) as Gravity;
	}
	
	void LateUpdate () {
		this.transform.up = worldCenter.transform.position - this.transform.position;
	}
}
