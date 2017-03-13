using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class MirrorToInput : GLMonoBehaviour {

	void Update () {
		float horizontal = Input.GetAxisRaw("Horizontal");
		if (horizontal < 0) {
			transform.SetScaleX(-1);
		} else if (horizontal > 0) {
			transform.SetScaleX(1);
		}
	}
}
