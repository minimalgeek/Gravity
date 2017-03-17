using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public enum Facing {
	LEFT, RIGHT
}

public class MirrorToInput : GLMonoBehaviour {

	public Facing facing = Facing.RIGHT;

	void Update () {
		float horizontal = Input.GetAxisRaw("Horizontal");
		if (horizontal < 0) {
			facing = Facing.LEFT;
			transform.SetScaleX(-1);
		} else if (horizontal > 0) {
			facing = Facing.RIGHT;
			transform.SetScaleX(1);
		}
	}
}
