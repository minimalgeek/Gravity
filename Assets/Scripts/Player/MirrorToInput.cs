using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorToInput : MonoBehaviour {

	void Update () {
		float horizontal = Input.GetAxisRaw("Horizontal");
		if (horizontal < 0) {
			transform.localScale = new Vector3(-1,1,1);
		} else if (horizontal > 0) {
			transform.localScale = new Vector3(1,1,1);
		}
	}
}
