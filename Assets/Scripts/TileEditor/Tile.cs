using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile {

	public float distanceFromCenter;
	public float angleInDegrees;
	public GameObject prefab;

	public Tile() {
	}

	public Tile(float dfc, float aid, GameObject p) {
		this.distanceFromCenter = dfc;
		this.angleInDegrees = aid;
		this.prefab = p;
	}

}
