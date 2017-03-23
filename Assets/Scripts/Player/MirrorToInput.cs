using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public enum Facing {
	LEFT, RIGHT
}

public class MirrorToInput : GLMonoBehaviour {

	private Facing previousFacing = Facing.RIGHT;
	public Facing facing = Facing.RIGHT;

	void Update () {
		float horizontal = Input.GetAxisRaw("Horizontal");
		if (horizontal < 0) {
			facing = Facing.LEFT;
		} else if (horizontal > 0) {
			facing = Facing.RIGHT;
		}

		if (previousFacing != facing) {
			previousFacing = facing;
			this.transform.FlipX();
		}
	}
}
